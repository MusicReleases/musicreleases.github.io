using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Base;
using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Artists;
using JakubKastner.SpotifyApi.Releases;
using JakubKastner.SpotifyApi.User;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal sealed class SpotifyReleaseService
(
	ISpotifyUserClient userApi,
	ISpotifyReleaseClient releaseApi,
	ISpotifyReleaseDbService releaseDb,
	ISpotifyArtistDbService artistDb,
	ISpotifyArtistReleaseDbService artistReleaseDb,
	ISpotifyUserUpdateDbService updateDb,
	ISpotifyReleaseState releaseState,
	ISpotifyArtistState artistState,
	IBackgroundTaskManagerService taskManager,
	ILoadingService loadingService
)
	: SpotifyBaseSyncServiceCore<SpotifyRelease, ReleaseGroup>(userApi, updateDb, taskManager, loadingService), ISpotifyReleaseService
{
	private readonly ISpotifyReleaseClient _releaseApi = releaseApi;
	private readonly ISpotifyReleaseDbService _releaseDb = releaseDb;
	private readonly ISpotifyArtistDbService _artistDb = artistDb;
	private readonly ISpotifyArtistReleaseDbService _artistReleaseDb = artistReleaseDb;
	private readonly ISpotifyReleaseState _releaseState = releaseState;
	private readonly ISpotifyArtistState _artistState = artistState;

	private ReleaseAggregation? _pendingAggregation;

	protected override BackgroundTaskType TaskType => BackgroundTaskType.ReleasesGet;
	protected override string TaskTitle => "Getting releases";
	protected override string GetTaskDescription(ReleaseGroup group) => $"Getting {group.ToFriendlyString()} from followed artists";
	protected override DateTime? GetLastSync(ReleaseGroup group) => _releaseState.LastSync.GetValueOrDefault(group);
	protected override bool GetIsDataInState(ReleaseGroup group) => _releaseState.Items.GetValueOrDefault(group) is not null;

	private static SpotifyDbUpdateType MapReleaseGroupToDbUpdateType(ReleaseGroup releaseGroup) => releaseGroup switch
	{
		ReleaseGroup.Albums => SpotifyDbUpdateType.ReleasesAlbums,
		ReleaseGroup.Tracks => SpotifyDbUpdateType.ReleasesTracks,
		ReleaseGroup.Appears => SpotifyDbUpdateType.ReleasesAppears,
		ReleaseGroup.Compilations => SpotifyDbUpdateType.ReleasesCompilations,
		ReleaseGroup.Podcasts => throw new NotSupportedException(),
		_ => throw new NotSupportedException(nameof(releaseGroup))
	};

	private sealed record ReleaseAggregation(List<SpotifyRelease> Releases, List<SpotifyArtist> Artists, List<SpotifyArtistReleaseEntity> Links);

	public async Task Get(ReleaseGroup releaseGroup, bool forceUpdate = false)
	{
		if (releaseGroup == ReleaseGroup.Podcasts)
		{
			throw new NotSupportedException();
		}

		await RunGet(releaseGroup, forceUpdate);
	}

	protected override async Task<bool> LoadFromDbToState(ReleaseGroup releaseGroup, string userId, bool forceUpdate, BackgroundTask task)
	{
		return await task.RunStep("Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			var releaseGroupString = releaseGroup.ToFriendlyString();
			task.BeginAutoSegments(5);

			var artists = _artistState.Items;

			if (artists is null)
			{
				// no data to sync
				return false;
			}

			var lastSync = await task.RunSegment($"db - get releases last sync (user-update) - {releaseGroupString}", async ct =>
			{
				var metaDbType = MapReleaseGroupToDbUpdateType(releaseGroup);
				return await _updateDb.Get(userId, metaDbType, ct);
			});

			if (artists.Count == 0)
			{
				await task.RunSegment($"state - set releases - {releaseGroupString}", async ct =>
				{
					_releaseState.Set(releaseGroup, [], lastSync);
				});

				// TODO db update - should sync calc
				return false;
			}

			var artistsCount = artists.Count;

			var releaseArtistPayloadIds = await task.RunSegment($"db - get release ids with artist ids (main& featured) from followed artists (artist-release) - {releaseGroupString} - artists: {artistsCount}", async ct =>
			{
				var artistIds = artists.Select(a => a.Id).ToList();

				return await _artistReleaseDb.GetArtistsByArtistIds(artistIds, releaseGroup, ct);
			});

			var releasesCount = releaseArtistPayloadIds.Count;

			var payloadArtists = await task.RunSegment($"db - get artists by ids (artist) - {releaseGroupString} - {releasesCount}", async ct =>
			{
				var allArtistIds = releaseArtistPayloadIds.SelectMany(p => p.MainArtistIds.Concat(p.FeaturedArtistIds)).ToHashSet();

				var artists = await _artistDb.GetByIds(allArtistIds, ct);
				var artistsDict = artists.ToDictionary(a => a.Id);

				var releasePayloads = releaseArtistPayloadIds.Select(p => new SpotifyArtistReleasePayload
				(
					p.Id,
					[.. p.MainArtistIds.Select(id => artistsDict[id])],
					[.. p.FeaturedArtistIds.Select(id => artistsDict[id])])
				);

				return releasePayloads.ToList();
			});

			var releases = await task.RunSegment($"db - get releases by ids (release) - {releaseGroupString} - {releasesCount}", async ct =>
			{
				var allReleaseIds = payloadArtists.Select(p => p.Id).ToHashSet();

				return await _releaseDb.GetByIds(payloadArtists, releaseGroup, ct);
			});

			var shouldSync = await task.RunSegment($"state - set releases - {releaseGroupString} - {releasesCount}", async ct =>
			{
				_releaseState.Set(releaseGroup, releases, lastSync);

				var shouldSync = ShouldSync(releaseGroup, forceUpdate);
				return shouldSync;
			});

			return shouldSync;
		});
	}

	protected override async Task<IReadOnlyCollection<SpotifyRelease>?> LoadFromApi(ReleaseGroup group, string userId, BackgroundTask task)
	{
		await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			var releaseGroupString = group.ToFriendlyString();

			// get artists from state
			var artists = _artistState.Items;

			if (artists is null)
			{
				return;
			}

			if (artists.Count == 0)
			{
				// TODO db update - should sync calc
				task.BeginAutoSegments(1);
				await task.RunSegment($"state - set releases - {releaseGroupString}", async ct =>
				{
					_releaseState.Set(group, [], DateTime.Now);
				});
				return;
			}

			// get stored releases from state
			var storedReleases = _releaseState.Items[group];
			var artistLastReleaseDate = storedReleases?.SelectMany
				(
					r => r.Artists.Select(a => new { ArtistId = a.Id, r.ReleaseDate })
				)
				.GroupBy(x => x.ArtistId)
				.ToDictionary
				(
					g => g.Key, g => g.Max(x => x.ReleaseDate)
				);

			const int artistBatchSize = 15;
			var artistList = artists.ToList();
			var total = artistList.Count;

			task.BeginAutoSegments(1);

			for (var start = 0; start < total; start += artistBatchSize)
			{

				var batchArtists = artistList.Skip(start).Take(artistBatchSize).ToList();

				var batchReleases = new List<SpotifyRelease>(256);
				var batchLinks = new List<SpotifyArtistReleaseEntity>(512);
				var batchAllArtists = new HashSet<SpotifyArtist>();

				// lock object
				var gate = new object();


				await RequestScheduler.Run(batchArtists, 3, async (artist, ct2) =>
				{
					ct.ThrowIfCancellationRequested();

					// get last release date for artist
					DateTime cutoff;
					if (!artist.New && artistLastReleaseDate is not null && artistLastReleaseDate.TryGetValue(artist.Id, out var lastDate))
					{
						// only for old artists
						cutoff = lastDate.AddDays(-7);
					}
					else
					{
						cutoff = DateTime.MinValue;
					}

					// get releases from api
					var apiReleases = await _releaseApi.GetByArtist(artist, group, cutoff, ct);

					if (apiReleases.Count == 0)
					{
						return;
					}


					lock (gate)
					{

						// save release to db
						batchReleases.AddRange(apiReleases);

						// save artists and links to db
						foreach (var release in apiReleases)
						{
							foreach (var a in release.Artists)
							{
								batchAllArtists.Add(a);
								batchLinks.Add(release.Id.ToArtistReleaseEntity(a.Id, ArtistReleaseRole.Main, release.ReleaseType));
							}

							foreach (var fa in release.FeaturedArtists)
							{
								batchAllArtists.Add(fa);
								batchLinks.Add(release.Id.ToArtistReleaseEntity(fa.Id, ArtistReleaseRole.Featured, release.ReleaseType));
							}
						}
					}
				}, ct, async (done, total) =>
				{
					if (done % 5 == 0 || done == total)
					{
						// progress
						task.SetSubProgress((start + done) / (double)artistList.Count, $"artists {start + done}/{artistList.Count}");
					}
					await Task.CompletedTask;
				});

				// save batch (DB + state merge)
				await SaveReleaseBatch(group, batchReleases, batchAllArtists.ToList(), batchLinks, task, ct);

				await Task.Yield();
			}

			await task.RunSegment($"db - save release last sync (update) - {releaseGroupString}", ct2 =>
			{
				var metaDbType = MapReleaseGroupToDbUpdateType(group);
				return _updateDb.Save(userId, metaDbType, ct2);
			});

			// filter
			await task.RunSegment($"state - filter releases by followed artists - {releaseGroupString}", _ =>
			{
				var followed = _artistState.Items?.Select(a => a.Id).ToHashSet() ?? [];
				var filtered = _releaseState.Items[group]
					.Where(r => r.Artists.Any(a => followed.Contains(a.Id)))
					.ToList();

				_releaseState.Set(group, filtered, _releaseState.LastSync[group]);
				return Task.CompletedTask;
			});
		});
		return null;
	}

	private async Task SaveReleaseBatch(ReleaseGroup group, IReadOnlyCollection<SpotifyRelease> releases, IReadOnlyCollection<SpotifyArtist> artists, IReadOnlyCollection<SpotifyArtistReleaseEntity> links, BackgroundTask task, CancellationToken ct)
	{
		if (releases.Count == 0)
		{
			return;
		}

		await task.RunSegment($"db - save releases (release) - {releases.Count}", async ct2 =>
		{
			await _releaseDb.Save(releases, true, ct2);
		});
		await task.RunSegment($"db - save artists (artist) - {artists.Count}", async ct2 =>
		{
			return _artistDb.Save(artists, true, ct2);
		});
		await task.RunSegment($"db - save artist-release links - {links.Count}", async ct2 =>
		{
			await _artistReleaseDb.Save(links, ct2);
		});

		//  add new releases
		_releaseState.Merge(group, releases, DateTime.Now);
	}

	protected override async Task SaveToDbAndState(ReleaseGroup group, IReadOnlyCollection<SpotifyRelease> newReleases, string userId, BackgroundTask task)
	{
		if (newReleases is null || _pendingAggregation is null)
		{
			return;
		}
		try
		{
			await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
			{
				task.BeginAutoSegments(5);

				var releaseGroupString = group.ToFriendlyString();

				// save to db
				var relasesCount = newReleases.Count;

				await task.RunSegment($"db - save releases (release) - {releaseGroupString} - {relasesCount}", async ct =>
				{
					await _releaseDb.Save(newReleases, true, ct);
				});

				var artistsCount = _pendingAggregation.Artists.Count;
				await task.RunSegment($"db - save artists from releases (artist) - {releaseGroupString} - {artistsCount}", async ct =>
				{
					await _artistDb.Save(_pendingAggregation.Artists, true, ct);
				});

				var linksCount = _pendingAggregation.Links.Count;
				await task.RunSegment($"db - save release artists (artist-release) - {releaseGroupString} - {linksCount}", async ct =>
				{
					await _artistReleaseDb.Save(_pendingAggregation.Links, ct);
				});

				// update meta db
				await task.RunSegment($"db - save release last sync (update) - {releaseGroupString}", async ct =>
				{
					var metaDbType = MapReleaseGroupToDbUpdateType(group);
					await _updateDb.Save(userId, metaDbType, ct);
				});

				// update state
				await task.RunSegment($"state - set releases - {releaseGroupString} - {relasesCount}", async ct =>
				{
					_releaseState.Merge(group, newReleases, DateTime.Now);

					if (_artistState.Items is not null)
					{
						var followedArtistIds = _artistState.Items.Select(a => a.Id).ToHashSet();

						var filtered = _releaseState.Items[group].Where(r => r.Artists.Any(a => followedArtistIds.Contains(a.Id))).ToList();

						_releaseState.Set(group, filtered, _releaseState.LastSync[group]);
					}

				});
			});
		}
		finally
		{
			_pendingAggregation = null;
		}
	}
}