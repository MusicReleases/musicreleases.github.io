using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Extensions;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistService(ISpotifyUserClient userApi, ISpotifyArtistClient artistApi, ISpotifyArtistDbService artistDb, IDbSpotifyUserArtistService userArtistDb, IDbSpotifyUserUpdateService updateDb, ISpotifyArtistState artistState, IBackgroundTaskManagerService taskManager, ILoadingService loadingService) : ISpotifyArtistService
{
	private readonly ISpotifyUserClient _userApi = userApi;
	private readonly ISpotifyArtistClient _artistApi = artistApi;
	private readonly ISpotifyArtistDbService _artistDb = artistDb;
	private readonly IDbSpotifyUserArtistService _useArtistDb = userArtistDb;
	private readonly IDbSpotifyUserUpdateService _updateDb = updateDb;
	private readonly ISpotifyArtistState _artistState = artistState;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ILoadingService _loadingService = loadingService;

	private const SpotifyDbUpdateType _updateDbType = SpotifyDbUpdateType.Artists;


	public async Task Get(bool forceUpdate = false)
	{
		var taskType = BackgroundTaskType.ArtistsGet;


		if (_loadingService.IsLoading(taskType))
		{
			// TODO await existing task or stop it
			return;
		}

		var isInState = _artistState.FollowedArtists is not null;

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

		await _taskManager.Run(taskType, "Geting artists", "Getting followed artists", async task =>
		{
			var userId = _userApi.GetUserIdRequired();

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
		var lastSync = _artistState.LastSync ?? DateTime.MinValue;
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
				return await _updateDb.Get(userId, _updateDbType, ct);
			});

			var artistIds = await task.RunSegment("db - get followed artist ids (user-artist)", async ct =>
			{
				return await _useArtistDb.GetFollowedIds(userId, ct);
			});

			var artistsCount = artistIds.Count;

			if (artistsCount == 0)
			{
				await task.RunSegment("state - set followed artists", async ct =>
				{
					_artistState.SetFollowed([], lastSync);
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
				_artistState.SetFollowed(artists, lastSync);

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
			var apiArtists = await task.RunSegment($"api - get followed artists", _artistApi.GetFollowed);

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
				await _useArtistDb.SetFollowed(userId, apiIds, ct);
			});

			// save to update db
			await task.RunSegment("db - save artists last sync (update)", async ct =>
			{
				await _updateDb.Save(userId, _updateDbType, ct);
			});

			// update ui
			await task.RunSegment($"state - set followed artists - {apiArtistsCount}", async ct =>
			{
				_artistState.SetFollowed(apiArtists, DateTime.Now);
			});
		});
	}
}