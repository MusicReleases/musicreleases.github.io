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

public class SpotifyArtistService(ISpotifyApiUserService spotifyApiUserService, IApiArtistClient api, IDbSpotifyArtistService artistDb, IDbSpotifyUserArtistService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyArtistState state, IBackgroundTaskManagerService taskManager) : ISpotifyArtistService
{
	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;
	private readonly IApiArtistClient _api = api;
	private readonly IDbSpotifyArtistService _artistDb = artistDb;
	private readonly IDbSpotifyUserArtistService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyArtistState _state = state;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;

	private CancellationTokenSource? _cts;

	public async Task Get(bool forceUpdate = false)
	{
		// cancel any ongoing sync
		Cancel();
		_cts = new CancellationTokenSource();
		var ct = _cts.Token;

		try
		{
			var isInState = _state.SortedFollowedArtists.Any();

			if (isInState)
			{
				// calculate last sync
				var shouldSync = ShouldSync(forceUpdate);

				if (!shouldSync)
				{
					// no need to sync
					return;
				}
			}

			await _taskManager.Run(BackgroundTaskType.Artists, "Geting artists", "Getting loggedin user artists", async task =>
			{
				var userId = _spotifyApiUserService.GetUserIdRequired();


				if (!isInState)
				{
					// load data from db to state
					await using (await task.BeginStepAsync("Loading from DB", BackgroundTaskCategory.GetDb, ct))
					{
						await LoadFromDbToState(userId, task, ct);
					}

					// calculate last sync
					var shouldSync = ShouldSync(forceUpdate);

					if (!shouldSync)
					{
						// no need to sync
						return;
					}
				}

				// load from api
				List<SpotifyArtist> apiArtists;
				await using (await task.BeginStepAsync("Loading from API", BackgroundTaskCategory.GetApi, ct))
				{
					apiArtists = await LoadFromApi(task, ct);
				}

				// save api data to db and state
				await using (await task.BeginStepAsync("Saving to DB", BackgroundTaskCategory.SaveDb, ct))
				{
					await SaveToDbAndState(apiArtists, userId, task, ct);
				}
			});
		}
		catch (OperationCanceledException)
		{
			// cancelled
		}
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

	private async Task LoadFromDbToState(string userId, BackgroundTask task, CancellationToken ct)
	{
		task.SetSubProgress(0.00, "db.get.last-sync");
		var lastSync = await _metaDb.Get(userId, SpotifyDbUpdateType.Artists);

		task.SetSubProgress(0.20, "db.get.user-artists");
		var artistIds = await _linkDb.GetFollowedIds(userId);

		if (artistIds.Count == 0)
		{
			_state.SetFollowed([], lastSync);
			task.SetSubProgress(1.00, "return.empty");
			return;
		}

		task.SetSubProgress(0.60, "db.get.artists.by-ids");
		var artists = await _artistDb.GetByIds(artistIds);

		_state.SetFollowed(artists, lastSync);
		task.SetSubProgress(1.00, "db.get.artists.done");
	}

	private async Task<List<SpotifyArtist>> LoadFromApi(BackgroundTask task, CancellationToken ct)
	{
		// get saved playlists from api
		task.SetSubProgress(0.00, "api.get.artists.start");
		var apiArtists = await _api.GetFollowed(ct);

		ct.ThrowIfCancellationRequested();
		task.SetSubProgress(1.00, $"api.get.artists.done.count-{apiArtists.Count}");

		return apiArtists;
	}

	private async Task SaveToDbAndState(List<SpotifyArtist> apiArtists, string userId, BackgroundTask task, CancellationToken ct)
	{
		// save to playlists db
		task.SetSubProgress(0.10, "db.save.artists");
		await _artistDb.Save(apiArtists);

		// save to user-playlist db
		task.SetSubProgress(0.60, "db.save.user-artists");
		var apiIds = apiArtists.Select(a => a.Id);
		await _linkDb.SetFollowed(userId, apiIds);

		// save to update db
		task.SetSubProgress(0.85, "db.save.last-sync");
		await _metaDb.Save(userId, SpotifyDbUpdateType.Artists);

		// update ui
		_state.SetFollowed(apiArtists, DateTime.Now);
		task.SetSubProgress(1.00, "db.save.playlists.done");
	}

	public void Cancel()
	{
		_cts?.Cancel();
	}
}