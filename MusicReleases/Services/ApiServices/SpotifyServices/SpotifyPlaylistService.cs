using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyPlaylistService(ISpotifyApiUserService spotifyApiUserService, IApiPlaylistClient api, IDbSpotifyPlaylistService playlistsDb, IDbSpotifyUserPlaylistService linkDb, IDbSpotifyUpdateService metaDb, ISpotifyPlaylistState state, ISpotifyTaskManagerService taskManager) : ISpotifyPlaylistService
{
	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;
	private readonly IApiPlaylistClient _api = api;
	private readonly IDbSpotifyPlaylistService _playlistDb = playlistsDb;
	private readonly IDbSpotifyUserPlaylistService _linkDb = linkDb;
	private readonly IDbSpotifyUpdateService _metaDb = metaDb;
	private readonly ISpotifyPlaylistState _state = state;
	private readonly ISpotifyTaskManagerService _taskManager = taskManager;

	private CancellationTokenSource? _cts;

	public async Task LoadAndSync(bool forceUpdate = false)
	{
		Cancel();
		_cts = new CancellationTokenSource();
		var token = _cts.Token;

		try
		{
			var userId = _spotifyApiUserService.GetUserIdRequired();
			// load data from db to state
			await LoadFromDbToState(userId);
			Console.WriteLine("last sync get");
			var lastSync = await _metaDb.Get(userId, SpotifyDbUpdateType.Playlists);
			Console.WriteLine("last sync  - " + lastSync);
			var shouldSync = forceUpdate || (DateTime.Now - lastSync).TotalHours > 24;
			if (shouldSync)
			{
				await _taskManager.Run("Getting playlists", async (t) =>
				{
					await SyncProcess(userId, t, token);
				});
			}
		}
		catch (OperationCanceledException)
		{
			// cancelled}
		}
	}
	private async Task LoadFromDbToState(string userId)
	{
		var orderMap = await _linkDb.GetUserPlaylistOrder(userId);

		if (orderMap.Count == 0)
		{
			_state.SetPlaylists([]);
			return;
		}

		// load playlists
		var playlists = await _playlistDb.GetByIds(orderMap.Keys);

		// sort by order (unkown to end)
		var sortedPlaylists = playlists.OrderBy(p => orderMap.GetValueOrDefault(p.Id, int.MaxValue)).ToList();

		_state.SetPlaylists(sortedPlaylists);
	}

	private async Task SyncProcess(string userId, SpotifyBackgroundTask task, CancellationToken ct)
	{
		// get saved playlists from api
		task.Status = "Getting playlists from api...";
		var apiPlaylists = await _api.GetUserPlaylists(ct);
		ct.ThrowIfCancellationRequested();

		task.Status = $"Saving {apiPlaylists.Count} playlists to local database...";

		// save to playlists db
		await _playlistDb.Save(apiPlaylists);

		// save to user-playlist db
		await _linkDb.SetUserPlaylists(userId, apiPlaylists.Select(p => p.Id));

		// save to update db
		await _metaDb.Save(userId, SpotifyDbUpdateType.Playlists);

		// update ui
		task.Status = "Displaying playlists...";
		_state.SetPlaylists(apiPlaylists);
	}

	public async Task CreatePlaylist(string name)
	{
		await _taskManager.Run($"Creating playlist '{name}'", async (task) =>
		{
			var userId = _spotifyApiUserService.GetUserIdRequired();

			task.Status = "Sending request to Spotify...";
			var newPlaylist = await _api.CreatePlaylist(userId, name);

			task.Status = "Saving playlist to local database...";
			await _playlistDb.Add(newPlaylist);
			await _linkDb.AddUserPlaylist(userId, newPlaylist.Id, 0);

			task.Status = "Displaying playlist...";
			_state.Add(newPlaylist);
		});
	}

	public async Task AddTracks(string playlistId, List<string> trackUris, bool positionTop)
	{
		await _taskManager.Run("Adding tracks to playlist", async (task) =>
		{
			task.Status = "Sending request to Spotify...";
			var snapshotId = await _api.AddTracksToPlaylist(playlistId, trackUris, positionTop);

			task.Status = "Updating local snapshot...";
			await UpdateSnapshot(playlistId, snapshotId);
		});
	}

	public async Task RemoveTracks(string playlistId, List<string> trackUris)
	{
		await _taskManager.Run("Removing tracks from playlist", async (task) =>
		{
			task.Status = "Sending request to Spotify...";
			var snapshotId = await _api.RemoveTracksFromPlaylist(playlistId, trackUris);

			task.Status = "Updating local snapshot...";
			await UpdateSnapshot(playlistId, snapshotId);
		});
	}

	private async Task UpdateSnapshot(string playlistId, string snapshotId)
	{
		await _playlistDb.UpdateSnapshot(playlistId, snapshotId);

		var playlist = _state.GetById(playlistId);
		if (playlist != null)
		{
			playlist.SnapshotId = snapshotId;
		}
	}

	public void Cancel() => _cts?.Cancel();
}
