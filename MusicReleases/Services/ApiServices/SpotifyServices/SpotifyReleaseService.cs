using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Extensions;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Artists.Releases;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

internal sealed class SpotifyReleaseService(ISpotifyUserClient spotifyUserClient, ISpotifyReleaseClient api, IDbSpotifyReleaseService releaseDb, ISpotifyArtistDbService artistDb, ISpotifyArtistReleaseDbService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyArtistState artistState, ISpotifyReleaseState state, IBackgroundTaskManagerService taskManager, ILoadingService loadingservice) : ISpotifyReleaseService
{
	private readonly ISpotifyUserClient _spotifyUserClient = spotifyUserClient;
	private readonly ISpotifyReleaseClient _api = api;
	private readonly IDbSpotifyReleaseService _releaseDb = releaseDb;
	private readonly ISpotifyArtistDbService _artistDb = artistDb;
	private readonly ISpotifyArtistReleaseDbService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyArtistState _artistState = artistState;
	private readonly ISpotifyReleaseState _state = state;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ILoadingService _loadingservice = loadingservice;


	public async Task Get(ReleaseEnums releaseGroup, bool forceUpdate = false)
	{
		if (releaseGroup == ReleaseEnums.Podcasts)
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
				var shouldSync = await LoadFromDbToState(releaseGroup, userId, forceUpdate, task);

				if (!shouldSync)
				{
					// synced
					return;
				}
			}

			// load from api
			var releaseAggregation = await LoadFromApi(releaseGroup, task);

			if (releaseAggregation is null)
			{
				// no artists followed - end
				return;
			}

			// save api data to db and state
			await SaveToDbAndState(releaseGroup, releaseAggregation, userId, task);
		});
	}

	private bool ShouldSync(ReleaseEnums releaseGroup, bool forceUpdate)
	{
		if (forceUpdate)
		{
			return true;
		}
		var lastSync = _state.LastSyncByType.GetValueOrDefault(releaseGroup);
		var shouldSync = (DateTime.Now - lastSync).TotalHours > 24;

		return shouldSync;
	}

	private async Task<bool> LoadFromDbToState(ReleaseEnums releaseGroup, string userId, bool forceUpdate, BackgroundTask task)
	{
		return await task.RunStep("Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			var releaseGroupString = releaseGroup.ToFriendlyString();
			task.BeginAutoSegments(4);

			var lastSync = await task.RunSegment($"db - get releases last sync (user-update) - {releaseGroupString}", async ct =>
			{
				var metaDbType = MapToDbUpdateType(releaseGroup);
				return await _metaDb.Get(userId, metaDbType, ct);
			});

			var artists = _artistState.FollowedArtists;

			if (artists is null || artists.Count == 0)
			{
				await task.RunSegment($"state - set releases - {releaseGroupString}", async ct =>
				{
					_state.Set(releaseGroup, [], lastSync);
				});

				// TODO should sync calc
				return true;
			}
			var artistsCount = artists.Count;

			var releaseIds = await task.RunSegment($"db - get release ids from artists (artist-release) - {releaseGroupString} - artists: {artistsCount}", async ct =>
			{
				var artistIds = artists.Select(a => a.Id);
				var artistRole = EnumReleaseTypeExtensions.MapReleaseRoleFromGroup(releaseGroup);

				return await _linkDb.GetReleaseIds(artistIds, artistRole, ct);
			});

			var releasesCount = releaseIds.Count;

			var releases = await task.RunSegment($"db - get releases by ids (release) - {releaseGroupString} - {releasesCount}", async ct =>
			{
				return await _releaseDb.GetByIds(releaseIds, releaseGroup, ct);
			});

			var shouldSync = await task.RunSegment($"state - set releases - {releaseGroupString} - {releasesCount}", async ct =>
			{
				_state.Set(releaseGroup, releases, lastSync);

				var shouldSync = ShouldSync(releaseGroup, forceUpdate);
				return shouldSync;
			});
			return shouldSync;
		});
	}

	private async Task<ReleaseAggregation?> LoadFromApi(ReleaseEnums releaseGroup, BackgroundTask task)
	{
		var releaseGroupString = releaseGroup.ToFriendlyString();

		return await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			// get artists from state
			var artists = _artistState.FollowedArtists;

			if (artists is null || artists.Count == 0)
			{
				// TODO should sync calc
				task.BeginAutoSegments(1);
				await task.RunSegment($"state - set releases - {releaseGroup.ToFriendlyString()}", async ct =>
				{
					_state.Set(releaseGroup, [], DateTime.Now);
				});
				return null;
			}
			var artistsCount = artists.Count;

			var allReleasesToSave = new HashSet<SpotifyRelease>();
			var allLinksToSave = new HashSet<SpotifyArtistReleaseEntity>();
			var allArtistsToSave = new HashSet<SpotifyArtist>();

			task.BeginAutoSegments(artistsCount);
			var i = 1;

			foreach (var artist in artists)
			{
				ct.ThrowIfCancellationRequested();

				await task.RunSegment($"api - get releases for artist {artist.Name} - {releaseGroupString} - {i} / {artistsCount}", async ct =>
				{
					// get releases from api
					var apiReleases = await _api.GetByArtist(artist, releaseGroup, ct);

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
							allLinksToSave.Add(release.Id.ToArtistReleaseEntity(releaseArtist.Id, ArtistReleaseRole.Main));
						}

						foreach (var featArtist in release.FeaturedArtists)
						{
							ct.ThrowIfCancellationRequested();
							allArtistsToSave.Add(featArtist);
							allLinksToSave.Add(release.Id.ToArtistReleaseEntity(featArtist.Id, ArtistReleaseRole.Featured));
						}
					}
				});

				i++;
			}

			var releaseAggregation = new ReleaseAggregation(allReleasesToSave.ToList(), allArtistsToSave.ToList(), allLinksToSave.ToList());

			return releaseAggregation;
		});
	}

	private async Task SaveToDbAndState(ReleaseEnums releaseGroup, ReleaseAggregation releaseAggregation, string userId, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(5);

			var releaseGroupString = releaseGroup.ToFriendlyString();

			// save to db
			var relasesCount = releaseAggregation.Releases.Count;

			await task.RunSegment($"db - save releases (release) - {releaseGroupString} - {relasesCount}", async ct =>
			{
				await _releaseDb.Save(releaseAggregation.Releases, ct);
			});

			var artistsCount = releaseAggregation.Artists.Count;
			await task.RunSegment($"db - save artists from releases (artist) - {releaseGroupString} - {artistsCount}", async ct =>
			{
				await _artistDb.Save(releaseAggregation.Artists, ct);
			});

			var linksCount = releaseAggregation.Links.Count;
			await task.RunSegment($"db - save release artists (artist-release) - {releaseGroupString} - {linksCount}", async ct =>
			{
				await _linkDb.Save(releaseAggregation.Links, ct);
			});

			// update meta db
			await task.RunSegment($"db - save release last sync (update) - {releaseGroupString}", async ct =>
			{
				var metaDbType = MapToDbUpdateType(releaseGroup);
				await _metaDb.Save(userId, metaDbType, ct);
			});

			// update state
			await task.RunSegment($"state - set releases - {releaseGroupString} - {relasesCount}", async ct =>
			{
				_state.Set(releaseGroup, releaseAggregation.Releases, DateTime.Now);
			});
		});
	}

	private static SpotifyDbUpdateType MapToDbUpdateType(ReleaseEnums releasesType) => releasesType switch
	{
		ReleaseEnums.Albums => SpotifyDbUpdateType.ReleasesAlbums,
		ReleaseEnums.Tracks => SpotifyDbUpdateType.ReleasesTracks,
		ReleaseEnums.Appears => SpotifyDbUpdateType.ReleasesAppears,
		ReleaseEnums.Compilations => SpotifyDbUpdateType.ReleasesCompilations,
		ReleaseEnums.Podcasts => throw new NotSupportedException(),
		_ => throw new NotSupportedException(nameof(releasesType))
	};

	private sealed record ReleaseAggregation(List<SpotifyRelease> Releases, List<SpotifyArtist> Artists, List<SpotifyArtistReleaseEntity> Links);

}