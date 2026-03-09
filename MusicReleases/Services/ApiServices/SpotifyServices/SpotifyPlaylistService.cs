using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyPlaylistService(ISpotifyApiUserService spotifyApiUserService, IApiPlaylistClient api, IDbSpotifyPlaylistService playlistsDb, IDbSpotifyUserPlaylistService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyPlaylistState state, ISpotifyTaskManagerService taskManager, ISettingsService settingsService) : ISpotifyPlaylistService
{
	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;
	private readonly IApiPlaylistClient _api = api;
	private readonly IDbSpotifyPlaylistService _playlistDb = playlistsDb;
	private readonly IDbSpotifyUserPlaylistService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyPlaylistState _state = state;
	private readonly ISpotifyTaskManagerService _taskManager = taskManager;
	private readonly ISettingsService _settingsService = settingsService;

	private CancellationTokenSource? _cts;

	public async Task LoadAndSync(bool forceUpdate = false)
	{
		Cancel();
		_cts = new CancellationTokenSource();
		var token = _cts.Token;

		try
		{
			var userId = _spotifyApiUserService.GetUserIdRequired();

			var isInState = _state.Playlists is not null;

			if (!isInState)
			{
				// load data from db to state
				await LoadFromDbToState(userId);
			}

			var lastSync = _state.LastSync ?? DateTime.MinValue;
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

		Console.WriteLine("last sync get");
		var lastSync = await _metaDb.Get(userId, SpotifyDbUpdateType.Playlists);
		Console.WriteLine("last sync  - " + lastSync);

		if (orderMap.Count == 0)
		{
			_state.SetPlaylists([], lastSync);
			return;
		}

		// load playlists
		var playlists = await _playlistDb.GetByIds(orderMap.Keys);

		// sort by order (unkown to end)
		var sortedPlaylists = playlists.OrderBy(p => orderMap.GetValueOrDefault(p.Id, int.MaxValue)).ToList();


		_state.SetPlaylists(sortedPlaylists, lastSync);
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
		_state.SetPlaylists(apiPlaylists, DateTime.Now);
	}

	public async Task CreatePlaylist(string name)
	{
		await _taskManager.Run($"Creating playlist '{name}'", async (task) =>
		{
			var userId = _spotifyApiUserService.GetUserIdRequired();
			var addToProfile = _settingsService.UserSettings.PlaylistAddToProfile;

			task.Status = "Sending request to Spotify...";
			var newPlaylist = await _api.CreatePlaylist(userId, name, addToProfile);

			task.Status = "Saving playlist to local database...";
			await _playlistDb.Add(newPlaylist);
			await _linkDb.AddUserPlaylist(userId, newPlaylist.Id, 0);

			task.Status = "Displaying playlist...";
			_state.Add(newPlaylist);
		});
	}

	public async Task AddTrack(string playlistId, SpotifyTrack track, bool positionTop)
	{
		await AddTracks(playlistId, [track], positionTop);
	}

	public async Task RemoveTrack(string playlistId, SpotifyTrack track)
	{
		await RemoveTracks(playlistId, [track]);
	}

	public async Task AddTracks(string playlistId, IEnumerable<SpotifyTrack> tracks, bool positionTop)
	{
		var tracksList = tracks.ToList();
		if (tracksList.Count == 0)
		{
			return;
		}

		var playlist = _state.GetById(playlistId) ?? throw new InvalidOperationException($"Playlist not found in state");
		var trackLabel = tracksList.Count == 1 ? "" : "s";

		await _taskManager.Run($"Adding {tracksList.Count} track{trackLabel} to playlist '{playlist.Name}'", async (task) =>
		{
			task.Status = "Sending request to Spotify...";

			var trackUris = tracksList.Select(t => t.UrlApp).ToList();
			var snapshotId = await _api.AddTracksToPlaylist(playlist.Id, trackUris, positionTop);

			var ids = tracksList.Select(t => t.Id).ToList();
			// TODO save to db

			task.Status = "Updating local snapshot...";
			await UpdateLocalPlaylistAfterAdd(playlist, snapshotId, ids);
		});
	}
	public async Task RemoveTracks(string playlistId, IEnumerable<SpotifyTrack> tracks)
	{
		var tracksList = tracks.ToList();
		if (tracksList.Count == 0)
		{
			return;
		}

		var playlist = _state.GetById(playlistId) ?? throw new InvalidOperationException("Playlist not found in state");
		var trackLabel = tracksList.Count == 1 ? "" : "s";

		await _taskManager.Run($"Removing {tracksList.Count} track{trackLabel} from playlist '{playlist.Name}'", async (task) =>
		{
			task.Status = "Sending request to Spotify...";
			var trackUris = tracksList.Select(t => t.UrlApp).ToList();
			var snapshotId = await _api.RemoveTracksFromPlaylist(playlistId, trackUris);

			// TODO save to db

			task.Status = "Updating local snapshot...";
			var trackIds = tracksList.Select(t => t.Id).ToList();
			await UpdateLocalPlaylistAfterRemove(playlist, snapshotId, trackIds);
		});
	}

	private async Task UpdateLocalPlaylistAfterAdd(SpotifyPlaylist playlist, string snapshotId, List<string> trackIds)
	{
		// update snapshot in db
		await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId);

		// update state
		if (playlist is not null)
		{
			playlist.SnapshotId = snapshotId;

			foreach (var trackId in trackIds)
			{
				if (playlist.Tracks is ICollection<string> tracks)
				{
					tracks.Add(trackId);
				}
			}
		}
	}

	private async Task UpdateLocalPlaylistAfterRemove(SpotifyPlaylist playlist, string snapshotId, List<string> trackIds)
	{
		// update snapshot in db
		await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId);

		// update state
		if (playlist is not null)
		{
			playlist.SnapshotId = snapshotId;

			if (playlist.Tracks is ICollection<string> tracks)
			{
				foreach (var id in trackIds)
				{
					tracks.Remove(id);
				}
			}
		}
	}

	public void Cancel() => _cts?.Cancel();
}
