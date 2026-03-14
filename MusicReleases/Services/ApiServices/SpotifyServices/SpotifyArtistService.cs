using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Extensions;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

internal sealed class SpotifyArtistService(ISpotifyUserClient spotifyUserClient, ISpotifyArtistClient api, IDbSpotifyArtistService artistDb, IDbSpotifyUserArtistService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyArtistState state, IBackgroundTaskManagerService taskManager, ILoadingService loadingservice) : ISpotifyArtistService
{
	private readonly ISpotifyUserClient _spotifyUserClient = spotifyUserClient;
	private readonly ISpotifyArtistClient _api = api;
	private readonly IDbSpotifyArtistService _artistDb = artistDb;
	private readonly IDbSpotifyUserArtistService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyArtistState _state = state;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ILoadingService _loadingservice = loadingservice;


	public async Task Get(bool forceUpdate = false)
	{
		if (_loadingservice.IsLoading(BackgroundTaskType.ArtistsGet))
		{
			return;
		}

		var isInState = _state.SortedFollowedArtists.Any();

		if (isInState)
		{
			// calculate last sync
			var shouldSync = ShouldSync(forceUpdate);

			if (!shouldSync)
			{
				// synced
				return;
			}
		}

		await _taskManager.Run(BackgroundTaskType.ArtistsGet, "Geting artists", "Getting followed artists", async task =>
		{
			var userId = _spotifyUserClient.GetUserIdRequired();

			if (!isInState)
			{
				// load data from db to state
				var shouldSync = await LoadFromDbToState(userId, forceUpdate, task);

				if (!shouldSync)
				{
					// synced
					return;
				}
			}

			// load from api
			var apiArtists = await LoadFromApi(task);

			// save api data to db and state
			await SaveToDbAndState(apiArtists, userId, task);
		});
	}

	private bool ShouldSync(bool forceUpdate)
	{
		if (forceUpdate)
		{
			return true;
		}
		var lastSync = _state.LastSync ?? DateTime.MinValue;
		var shouldSync = (DateTime.Now - lastSync).TotalHours > 24;

		return shouldSync;
	}

	private async Task<bool> LoadFromDbToState(string userId, bool forceUpdate, BackgroundTask task)
	{
		return await task.RunStep("Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			task.BeginAutoSegments(5);

			var lastSync = await task.RunSegment("db - get artists last sync (user-update)", async ct =>
			{
				return await _metaDb.Get(userId, SpotifyDbUpdateType.Artists, ct);
			});

			var artistIds = await task.RunSegment("db - get followed artist ids (user-artist)", async ct =>
			{
				return await _linkDb.GetFollowedIds(userId, ct);
			});

			var artistsCount = artistIds.Count;

			if (artistsCount == 0)
			{
				await task.RunSegment("state - set followed artists", async ct =>
				{
					_state.SetFollowed([], lastSync);
				});

				// TODO should sync calc
				return true;
			}

			var artists = await task.RunSegment($"db - get artists by ids (artists) - {artistsCount}", async ct =>
			{
				return await _artistDb.GetByIds(artistIds, ct);
			});

			var shouldSync = await task.RunSegment($"state - set followed artists - {artistsCount}", async ct =>
			{
				_state.SetFollowed(artists, lastSync);

				// calculate last sync
				var shouldSync = ShouldSync(forceUpdate);
				return shouldSync;
			});

			return shouldSync;
		});
	}

	private async Task<List<SpotifyArtist>> LoadFromApi(BackgroundTask task)
	{
		return await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			task.BeginAutoSegments(1);

			// get saved playlists from api
			var apiArtists = await task.RunSegment($"api - get followed artists", _api.GetFollowed);

			return apiArtists;
		});
	}

	private async Task SaveToDbAndState(List<SpotifyArtist> apiArtists, string userId, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(4);

			var apiArtistsCount = apiArtists.Count;

			// save to playlists db
			await task.RunSegment($"db - save artists (artist) - {apiArtistsCount}", async ct =>
			{
				await _artistDb.Save(apiArtists, ct);
			});

			// save to user-playlist db
			await task.RunSegment($"db - save followed artists (user-artist) - {apiArtistsCount}", async ct =>
			{
				var apiIds = apiArtists.Select(a => a.Id);
				await _linkDb.SetFollowed(userId, apiIds, ct);
			});

			// save to update db
			await task.RunSegment("db - save artists last sync (update)", async ct =>
			{
				await _metaDb.Save(userId, SpotifyDbUpdateType.Artists, ct);
			});

			// update ui
			await task.RunSegment($"state - set followed artists - {apiArtistsCount}", async ct =>
			{
				_state.SetFollowed(apiArtists, DateTime.Now);
			});
		});
	}
}