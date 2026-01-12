using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyArtistService(ISpotifyApiUserService spotifyApiUserService, ISpotifyFilterService filterService, IApiArtistClient api, IDbSpotifyArtistService artistDb, IDbSpotifyUserArtistService linkDb, IDbSpotifyUpdateService metaDb, ISpotifyArtistState state, ISpotifyTaskManagerService taskManager) : ISpotifyArtistService
{
	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;
	private readonly ISpotifyFilterService _filterService = filterService;
	private readonly IApiArtistClient _api = api;
	private readonly IDbSpotifyArtistService _artistDb = artistDb;
	private readonly IDbSpotifyUserArtistService _linkDb = linkDb;
	private readonly IDbSpotifyUpdateService _metaDb = metaDb;
	private readonly ISpotifyArtistState _state = state;
	private readonly ISpotifyTaskManagerService _taskManager = taskManager;

	private CancellationTokenSource? _cts;

	public async Task Get(bool forceUpdate = false)
	{
		// cancel any ongoing sync
		Cancel();
		_cts = new CancellationTokenSource();
		var token = _cts.Token;

		try
		{
			var userId = _spotifyApiUserService.GetUserIdRequired();
			// load data from db to state
			await LoadFromDbToState(userId);
			Console.WriteLine("last sync get");
			var lastSync = await _metaDb.Get(userId, SpotifyDbUpdateType.Artists);
			Console.WriteLine("last sync  - " + lastSync);
			var shouldSync = forceUpdate || (DateTime.Now - lastSync).TotalHours > 24;

			if (shouldSync)
			{
				await _taskManager.Run("Getting artists", async (task) =>
				{
					await SyncProcess(userId, task, token);
				});
			}
		}
		catch (OperationCanceledException)
		{
			// cancelled
		}
	}

	private async Task LoadFromDbToState(string userId)
	{
		var artistIds = await _linkDb.GetFollowedIds(userId);

		if (artistIds.Count == 0)
		{
			return;
		}

		var artists = await _artistDb.GetByIds(artistIds);

		_state.SetFollowed(artists);

		_filterService.SetArtists(_state.SortedFollowedArtists.ToHashSet());
	}

	private async Task SyncProcess(string userId, SpotifyBackgroundTask task, CancellationToken ct)
	{
		// get followed artists from api
		task.Status = "Getting artists from api...";
		var apiArtists = await _api.GetFollowed(ct);
		ct.ThrowIfCancellationRequested();

		task.Status = $"Saving {apiArtists.Count} artists to local db...";

		// save to artist db
		await _artistDb.Save(apiArtists);
		ct.ThrowIfCancellationRequested();

		// save to user-artist db
		var apiIds = apiArtists.Select(a => a.Id);
		await _linkDb.SetFollowed(userId, apiIds);

		// save to update db
		await _metaDb.Save(userId, SpotifyDbUpdateType.Artists);

		// update ui
		task.Status = "Displaying artists...";
		_state.SetFollowed(apiArtists);

		_filterService.SetArtists(_state.SortedFollowedArtists.ToHashSet());
	}

	public void Cancel()
	{
		_cts?.Cancel();
	}
}