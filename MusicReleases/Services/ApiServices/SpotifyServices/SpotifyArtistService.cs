using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyArtistService(ISpotifyApiUserService spotifyApiUserService, IApiArtistClient api, IDbSpotifyArtistService artistDb, IDbSpotifyUserArtistService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyArtistState state, IBackgroundTaskManagerService taskManager, ILoadingService loadingservice) : ISpotifyArtistService
{
	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;
	private readonly IApiArtistClient _api = api;
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
			var userId = _spotifyApiUserService.GetUserIdRequired();

			if (!isInState)
			{
				// load data from db to state
				await LoadFromDbToState(userId, forceUpdate, task);
			}

			Console.WriteLine($"idiot");
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

		Console.WriteLine($"{lastSync.ToString("G")} - {shouldSync}");

		return shouldSync;
	}

	private async Task LoadFromDbToState(string userId, bool forceUpdate, BackgroundTask task)
	{
		await task.RunStep("Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			task.BeginAutoSegments(5);

			var lastSync = await task.SegmentAsync("db - get artists last sync (user-update)", async ct =>
			{
				return await _metaDb.Get(userId, SpotifyDbUpdateType.Artists, ct);
			});

			var artistIds = await task.SegmentAsync("db - get followed artist ids (user-artist)", async ct =>
			{
				return await _linkDb.GetFollowedIds(userId, ct);
			});


			if (artistIds.Count == 0)
			{
				await using (await task.NextSegment("state - set followed artists"))
				{
					_state.SetFollowed([], lastSync);
				}

				return;
			}

			var artists = await task.SegmentAsync("db - get artists by ids (artists)", async ct =>
			{
				return await _artistDb.GetByIds(artistIds, ct);
			});

			await using (await task.NextSegment("state - set followed artists"))
			{
				_state.SetFollowed(artists, lastSync);

				// calculate last sync
				var shouldSync = ShouldSync(forceUpdate);

				if (!shouldSync)
				{
					// synced
					Console.WriteLine($"end-task");
					task.EndTask();
					return;
				}


				Console.WriteLine($"but");
			}
			Console.WriteLine($"i am still");
		});
		Console.WriteLine($"here");
	}

	private async Task<List<SpotifyArtist>> LoadFromApi(BackgroundTask task)
	{
		return await task.StepAsync("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			task.BeginAutoSegments(1);

			// get saved playlists from api
			var apiArtists = await task.SegmentAsync($"api - get followed artists", _api.GetFollowed);

			return apiArtists;
		});
	}

	private async Task SaveToDbAndState(List<SpotifyArtist> apiArtists, string userId, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(4);

			// save to playlists db
			await using (await task.NextSegment("db - save artists (artist)"))
			{
				await _artistDb.Save(apiArtists, ct);
			}

			// save to user-playlist db
			await using (await task.NextSegment("db - save followed artists (user-artist)"))
			{
				var apiIds = apiArtists.Select(a => a.Id);
				await _linkDb.SetFollowed(userId, apiIds, ct);
			}

			// save to update db
			await using (await task.NextSegment("db - save artists last sync (update)"))
			{
				await _metaDb.Save(userId, SpotifyDbUpdateType.Artists, ct);
			}

			// update ui
			await using (await task.NextSegment("state - set followed artists"))
			{
				_state.SetFollowed(apiArtists, DateTime.Now);
			}
		});
	}
}