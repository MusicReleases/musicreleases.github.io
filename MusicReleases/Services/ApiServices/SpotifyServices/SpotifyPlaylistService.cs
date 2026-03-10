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
		var ct = _cts.Token;

		try
		{
			await _taskManager.Run("Geting playlists", BackgroundTaskType.Playlists, async task =>
			{
				var userId = _spotifyApiUserService.GetUserIdRequired();

				var isInState = _state.Playlists is not null;

				if (!isInState)
				{
					// load data from db to state
					await using (await task.BeginStepAsync("Loading from DB", BackgroundTaskCategory.GetDb))
					{
						await LoadFromDbToState(userId, task, ct);
					}
				}

				// calculate last sync
				var lastSync = _state.LastSync ?? DateTime.MinValue;
				var shouldSync = forceUpdate || (DateTime.Now - lastSync).TotalHours > 24;

				if (!shouldSync)
				{
					// end task (synced)
					return;
				}

				// load from api
				List<SpotifyPlaylist> apiPlaylists;
				await using (await task.BeginStepAsync("Loading from API", BackgroundTaskCategory.GetApi))
				{
					apiPlaylists = await LoadFromApi(task, ct);
				}

				// save api data to db and state
				await using (await task.BeginStepAsync("Saving to DB", BackgroundTaskCategory.SaveDb))
				{
					await SaveToDbAndState(apiPlaylists, userId, task, ct);
				}
			});
		}
		catch (OperationCanceledException)
		{
			// cancelled
		}
	}

	private async Task LoadFromDbToState(string userId, BackgroundTask task, CancellationToken ct)
	{
		task.SetSubProgress(0.00, "db.get.last-sync");
		var lastSync = await _metaDb.Get(userId, SpotifyDbUpdateType.Playlists);

		task.SetSubProgress(0.20, "db.get.user-playlists.order");
		var orderMap = await _linkDb.GetUserPlaylistOrder(userId);

		if (orderMap.Count == 0)
		{
			_state.SetPlaylists([], lastSync);
			task.SetSubProgress(1.00, "return.empty");
			return;
		}

		// load playlists
		task.SetSubProgress(0.50, "db.get.playlists.by-ids");
		var playlists = await _playlistDb.GetByIds(orderMap.Keys);

		// sort by order (unkown to end)
		task.SetSubProgress(0.90, "sort");
		var sortedPlaylists = playlists.OrderBy(p => orderMap.GetValueOrDefault(p.Id, int.MaxValue)).ToList();

		_state.SetPlaylists(sortedPlaylists, lastSync);
		task.SetSubProgress(1.00, "db.get.playlists.done");
	}

	private async Task<List<SpotifyPlaylist>> LoadFromApi(BackgroundTask task, CancellationToken ct)
	{
		// get saved playlists from api
		task.SetSubProgress(0.00, "api.get.playlists.start");
		var apiPlaylists = await _api.GetUserPlaylists(ct);

		ct.ThrowIfCancellationRequested();
		task.SetSubProgress(1.00, "api.get.playlists.done");

		return apiPlaylists;
	}

	private async Task SaveToDbAndState(List<SpotifyPlaylist> apiPlaylists, string userId, BackgroundTask task, CancellationToken ct)
	{
		// save to playlists db
		task.SetSubProgress(0.10, "db.save.playlists");
		await _playlistDb.Save(apiPlaylists);

		// save to user-playlist db
		task.SetSubProgress(0.60, "db.save.user-playlists");
		await _linkDb.SetUserPlaylists(userId, apiPlaylists.Select(p => p.Id));

		// save to update db
		task.SetSubProgress(0.85, "db.save.last-sync");
		await _metaDb.Save(userId, SpotifyDbUpdateType.Playlists);

		// update ui
		_state.SetPlaylists(apiPlaylists, DateTime.Now);
		task.SetSubProgress(1.00, "db.save.playlists.done");
	}


	public async Task CreatePlaylist(string name)
	{
		await _taskManager.Run($"Creating playlists '{name}'", BackgroundTaskType.Playlists, async task =>
		{
			var userId = _spotifyApiUserService.GetUserIdRequired();
			SpotifyPlaylist newPlaylist;

			await using (await task.BeginStepAsync("Sending API request", BackgroundTaskCategory.SaveApi))
			{
				task.SetSubProgress(0.00, "api.playlist.get.start");
				var addToProfile = _settingsService.UserSettings.PlaylistAddToProfile;

				newPlaylist = await _api.CreatePlaylist(userId, name, addToProfile);
				task.SetSubProgress(1.00, "api.playlist.get.done");

				task.AddLink("Open playlist", newPlaylist.UrlWeb, newPlaylist.UrlApp);

			}
			await using (await task.BeginStepAsync("Saving to DB", BackgroundTaskCategory.SaveDb))
			{
				task.SetSubProgress(0.00, "db.add.playlist");
				await _playlistDb.Add(newPlaylist);

				task.SetSubProgress(0.50, "db.add.user-playlist");
				await _linkDb.AddUserPlaylist(userId, newPlaylist.Id, 0);

				_state.Add(newPlaylist);
				task.SetSubProgress(1.00, "db.add.playlist.done");
			}
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

		var trackLabel = tracksList.Count == 1 ? "" : "s";

		var playlist = _state.GetById(playlistId) ?? throw new InvalidOperationException($"Playlist not found in state");

		await _taskManager.Run($"Adding {tracksList.Count} track{trackLabel} to playlist '{playlist.Name}'", BackgroundTaskType.PlaylistTracks, async (task) =>
		{
			string snapshotId;

			await using (await task.BeginStepAsync("Sending API request", BackgroundTaskCategory.SaveApi))
			{
				task.SetSubProgress(0.00, "api.playlist-tracks.set.start");
				var trackUris = tracksList.Select(t => t.UrlApp).ToList();
				snapshotId = await _api.AddTracksToPlaylist(playlist.Id, trackUris, positionTop);
				task.SetSubProgress(1.00, "api.playlist-tracks.set.done");

				task.AddLink("Open playlist", playlist.UrlWeb, playlist.UrlApp);
			}

			await using (await task.BeginStepAsync("Saving to DB", BackgroundTaskCategory.SaveDb))
			{
				task.SetSubProgress(0.00, "db.update.playlist.snapshot");

				var ids = tracksList.Select(t => t.Id).ToList();
				await UpdateLocalPlaylistAfterAdd(playlist, snapshotId, ids);

				// TODO playlists tracks add to db

				task.SetSubProgress(1.00, "db.update.playlist.snapshot.done");
			}
		});
	}
	public async Task RemoveTracks(string playlistId, IEnumerable<SpotifyTrack> tracks)
	{
		var tracksList = tracks.ToList();
		if (tracksList.Count == 0)
		{
			return;
		}
		var trackLabel = tracksList.Count == 1 ? "" : "s";

		var playlist = _state.GetById(playlistId) ?? throw new InvalidOperationException("Playlist not found in state");

		await _taskManager.Run($"Removing {tracksList.Count} track{trackLabel} from playlist '{playlist.Name}'", BackgroundTaskType.PlaylistTracks, async (task) =>
		{
			string snapshotId;

			await using (await task.BeginStepAsync("Sending API request", BackgroundTaskCategory.DeleteApi))
			{
				task.SetSubProgress(0.00, "api.playlist-tracks.remove.start");
				var trackUris = tracksList.Select(t => t.UrlApp).ToList();
				snapshotId = await _api.RemoveTracksFromPlaylist(playlistId, trackUris);

				task.SetSubProgress(1.00, "api.playlist-tracks.remove.done");

				task.AddLink("Open playlist", playlist.UrlWeb, playlist.UrlApp);
			}
			await using (await task.BeginStepAsync("Saving to DB", BackgroundTaskCategory.SaveDb))
			{
				task.SetSubProgress(0.00, "db.update.playlist.snapshot");

				var trackIds = tracksList.Select(t => t.Id).ToList();
				await UpdateLocalPlaylistAfterRemove(playlist, snapshotId, trackIds);

				// TODO playlists tracks remove from db

				task.SetSubProgress(1.00, "db.update.playlist.snapshot.done");
			}
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
