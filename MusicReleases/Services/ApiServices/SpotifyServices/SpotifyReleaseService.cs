using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services.Api;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyReleaseService(IApiUserClient spotifyUserClient, IApiReleaseClient api, IDbSpotifyReleaseService releaseDb, IDbSpotifyArtistService artistDb, IDbSpotifyArtistReleaseService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyArtistState artistState, ISpotifyReleaseState state, IBackgroundTaskManagerService taskManager, ILoadingService loadingservice) : ISpotifyReleaseService
{
	private readonly IApiUserClient _spotifyUserClient = spotifyUserClient;
	private readonly IApiReleaseClient _api = api;
	private readonly IDbSpotifyReleaseService _releaseDb = releaseDb;
	private readonly IDbSpotifyArtistService _artistDb = artistDb;
	private readonly IDbSpotifyArtistReleaseService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyArtistState _artistState = artistState;
	private readonly ISpotifyReleaseState _state = state;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ILoadingService _loadingservice = loadingservice;


	public async Task Get(ReleaseGroup releaseGroup, bool forceUpdate = false)
	{
		if (releaseGroup == ReleaseGroup.Podcasts)
		{
			// TODO podcasts
			throw new NotSupportedException();
		}

		if (_loadingservice.IsLoading(BackgroundTaskType.ReleasesGet))
		{
			return;
		}

		var isInState = _state.ReleasesByType.GetValueOrDefault(releaseGroup) is not null;

		if (isInState)
		{
			// calculate last sync
			var shouldSync = ShouldSync(releaseGroup, forceUpdate);

			if (!shouldSync)
			{
				// synced
				return;
			}
		}

		await _taskManager.Run(BackgroundTaskType.ReleasesGet, "Getting releases", $"Geting {releaseGroup.ToFriendlyString()} from followed artists", async task =>
		{
			var userId = _spotifyUserClient.GetUserIdRequired();

			if (!isInState)
			{
				// load data from db to state
				await LoadFromDbToState(releaseGroup, userId, forceUpdate, task);
			}

			// load from api
			var releaseAggregation = await LoadFromApi(releaseGroup, task);

			// save api data to db and state
			await SaveToDbAndState(releaseGroup, releaseAggregation, userId, task);
		});
	}

	private bool ShouldSync(ReleaseGroup releaseGroup, bool forceUpdate)
	{
		if (forceUpdate)
		{
			return true;
		}
		var lastSync = _state.LastSyncByType.GetValueOrDefault(releaseGroup);
		var shouldSync = (DateTime.Now - lastSync).TotalHours > 24;

		return shouldSync;
	}

	private async Task LoadFromDbToState(ReleaseGroup releaseGroup, string userId, bool forceUpdate, BackgroundTask task)
	{
		await task.RunStep("Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			var releaseGroupString = releaseGroup.ToFriendlyString();
			task.BeginAutoSegments(4);

			var lastSync = await task.SegmentAsync($"db - get releases last sync (user-update) - {releaseGroupString}", async ct =>
			{
				var metaDbType = MapToDbUpdateType(releaseGroup);
				return await _metaDb.Get(userId, metaDbType, ct);
			});

			var artists = _artistState.SortedFollowedArtists;
			if (artists.Count == 0)
			{
				await using (await task.NextSegment($"state - set releases - {releaseGroupString}"))
				{
					_state.Set(releaseGroup, [], lastSync);
				}
				return;
			}

			var releaseIds = await task.SegmentAsync($"db - get release ids from artists (artist-release) - {releaseGroupString}", async ct =>
			{
				var artistIds = artists.Select(a => a.Id);
				var artistRole = EnumReleaseTypeExtensions.MapReleaseRoleFromGroup(releaseGroup);

				return await _linkDb.GetReleaseIds(artistIds, artistRole, ct);
			});

			var releases = await task.SegmentAsync($"db - get releases by ids (release) - {releaseGroupString}", async ct =>
			{
				return await _releaseDb.GetByIds(releaseIds, releaseGroup, ct);
			});

			await using (await task.NextSegment($"state - set releases - {releaseGroupString}"))
			{
				_state.Set(releaseGroup, releases, lastSync);

				var shouldSync = ShouldSync(releaseGroup, forceUpdate);

				if (!shouldSync)
				{
					// synced
					task.EndTask();
					return;
				}
			}
		});
	}

	private async Task<ReleaseAggregation> LoadFromApi(ReleaseGroup releaseGroup, BackgroundTask task)
	{
		var releaseGroupString = releaseGroup.ToFriendlyString();

		return await task.StepAsync<ReleaseAggregation>("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			// get artists from state
			var artists = _artistState.SortedFollowedArtists;
			if (artists.Count == 0)
			{
				task.EndTask();
				return new([], [], []);
			}

			var allReleasesToSave = new HashSet<SpotifyRelease>();
			var allLinksToSave = new HashSet<SpotifyArtistReleaseEntity>();
			var allArtistsToSave = new HashSet<SpotifyArtist>();

			task.BeginAutoSegments(artists.Count);

			foreach (var artist in artists)
			{
				ct.ThrowIfCancellationRequested();

				await using (await task.NextSegment($"api - get releases for artist {artist.Name} - {releaseGroupString}"))
				{
					// get releases from api
					var apiReleases = await _api.GetByArtist(artist, releaseGroup, ct);

					if (apiReleases.Count == 0)
					{
						continue;
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
							allLinksToSave.Add(release.Id.ToArtistReleaseEntity(releaseArtist.Id, ArtistReleaseRole.Main));
						}

						foreach (var featArtist in release.FeaturedArtists)
						{
							ct.ThrowIfCancellationRequested();
							allArtistsToSave.Add(featArtist);
							allLinksToSave.Add(release.Id.ToArtistReleaseEntity(featArtist.Id, ArtistReleaseRole.Featured));
						}
					}
				}
			}

			var releaseAggregation = new ReleaseAggregation(allReleasesToSave.ToList(), allArtistsToSave.ToList(), allLinksToSave.ToList());

			return releaseAggregation;
		});
	}

	private async Task SaveToDbAndState(ReleaseGroup releaseGroup, ReleaseAggregation releaseAggregation, string userId, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(5);

			var releaseGroupString = releaseGroup.ToFriendlyString();

			// save to db
			await using (await task.NextSegment($"db - save releases (release) - {releaseGroupString}"))
			{
				await _releaseDb.Save(releaseAggregation.Releases, ct);
			}

			await using (await task.NextSegment($"db - save artists from releases (artist) - {releaseGroupString}"))
			{
				await _artistDb.Save(releaseAggregation.Artists, ct);
			}

			await using (await task.NextSegment($"db - save release artists (artist-release) - {releaseGroupString}"))
			{
				await _linkDb.Save(releaseAggregation.Links, ct);
			}

			// update meta db
			await using (await task.NextSegment($"db - save release last sync (update) - {releaseGroupString}"))
			{
				var metaDbType = MapToDbUpdateType(releaseGroup);
				await _metaDb.Save(userId, metaDbType, ct);
			}

			// update state
			await using (await task.NextSegment($"state - set releases - {releaseGroupString}"))
			{
				_state.Set(releaseGroup, releaseAggregation.Releases, DateTime.Now);
			}
		});
	}

	private static SpotifyDbUpdateType MapToDbUpdateType(ReleaseGroup releasesType) => releasesType switch
	{
		ReleaseGroup.Albums => SpotifyDbUpdateType.ReleasesAlbums,
		ReleaseGroup.Tracks => SpotifyDbUpdateType.ReleasesTracks,
		ReleaseGroup.Appears => SpotifyDbUpdateType.ReleasesAppears,
		ReleaseGroup.Compilations => SpotifyDbUpdateType.ReleasesCompilations,
		ReleaseGroup.Podcasts => throw new NotSupportedException(),
		_ => throw new NotSupportedException(nameof(releasesType))
	};

	private sealed record ReleaseAggregation(List<SpotifyRelease> Releases, List<SpotifyArtist> Artists, List<SpotifyArtistReleaseEntity> Links);

}