using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Extensions;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Base;
using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.SpotifyApi.Artists;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal sealed class SpotifyReleaseService(ISpotifyUserClient userApi, ISpotifyReleaseClient releaseApi, ISpotifyReleaseDbService releaseDb, ISpotifyArtistDbService artistDb, ISpotifyArtistReleaseDbService artistReleaseDb, IDbSpotifyUserUpdateService updateDb, ISpotifyReleaseState releaseState, ISpotifyArtistState artistState, IBackgroundTaskManagerService taskManager, ILoadingService loadingService) : SpotifyBaseSyncServiceCore<SpotifyRelease, ReleaseGroup>(userApi, updateDb, taskManager, loadingService), ISpotifyReleaseService
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

				// TODO should sync calc
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

	protected override async Task<IReadOnlyCollection<SpotifyRelease>?> LoadFromApi(ReleaseGroup group, BackgroundTask task)
	{
		return await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			var releaseGroupString = group.ToFriendlyString();

			// get artists from state
			var artists = _artistState.Items;

			if (artists is null)
			{
				return null;
			}

			if (artists.Count == 0)
			{
				// TODO should sync calc
				task.BeginAutoSegments(1);
				await task.RunSegment($"state - set releases - {releaseGroupString}", async ct =>
				{
					_releaseState.Set(group, [], DateTime.Now);
				});
				return null;
			}

			var allReleasesToSave = new HashSet<SpotifyRelease>();
			var allLinksToSave = new HashSet<SpotifyArtistReleaseEntity>();
			var allArtistsToSave = new HashSet<SpotifyArtist>();
			var i = 1;
			var artistsCount = artists.Count;

			task.BeginAutoSegments(artistsCount);

			foreach (var artist in artists)
			{
				ct.ThrowIfCancellationRequested();

				await task.RunSegment($"api - get releases for artist {artist.Name} - {releaseGroupString} - {i} / {artistsCount}", async ct =>
				{
					// get releases from api
					var apiReleases = await _releaseApi.GetByArtist(artist, group, ct);

					if (apiReleases.Count == 0)
					{
						return;
					}

					// save release to db
					allReleasesToSave.UnionWith(apiReleases);

					// save artists and links to db
					foreach (var release in apiReleases)
					{
						ct.ThrowIfCancellationRequested();

						foreach (var releaseArtist in release.Artists)
						{
							ct.ThrowIfCancellationRequested();

							allArtistsToSave.Add(releaseArtist);
							allLinksToSave.Add(release.Id.ToArtistReleaseEntity(releaseArtist.Id, ArtistReleaseRole.Main, release.ReleaseType));
						}

						foreach (var featArtist in release.FeaturedArtists)
						{
							ct.ThrowIfCancellationRequested();

							allArtistsToSave.Add(featArtist);
							allLinksToSave.Add(release.Id.ToArtistReleaseEntity(featArtist.Id, ArtistReleaseRole.Featured, release.ReleaseType));
						}
					}
				});

				i++;
			}

			_pendingAggregation = new([.. allReleasesToSave], [.. allArtistsToSave], [.. allLinksToSave]);

			return _pendingAggregation.Releases;
		});
	}

	protected override async Task SaveToDbAndState(ReleaseGroup group, IReadOnlyCollection<SpotifyRelease> _, string userId, BackgroundTask task)
	{
		if (_pendingAggregation is null)
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
				var relasesCount = _pendingAggregation.Releases.Count;

				await task.RunSegment($"db - save releases (release) - {releaseGroupString} - {relasesCount}", async ct =>
				{
					await _releaseDb.Save(_pendingAggregation.Releases, ct);
				});

				var artistsCount = _pendingAggregation.Artists.Count;
				await task.RunSegment($"db - save artists from releases (artist) - {releaseGroupString} - {artistsCount}", async ct =>
				{
					await _artistDb.Save(_pendingAggregation.Artists, ct);
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
					_releaseState.Set(group, _pendingAggregation.Releases, DateTime.Now);
				});
			});
		}
		finally
		{
			_pendingAggregation = null;
		}
	}
}