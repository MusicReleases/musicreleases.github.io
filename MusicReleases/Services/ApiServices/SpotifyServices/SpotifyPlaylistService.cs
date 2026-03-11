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

public class SpotifyPlaylistService(ISpotifyApiUserService spotifyApiUserService, IApiPlaylistClient api, IDbSpotifyPlaylistService playlistsDb, IDbSpotifyUserPlaylistService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyPlaylistState state, IBackgroundTaskManagerService taskManager, ISettingsService settingsService) : ISpotifyPlaylistService
{
	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;
	private readonly IApiPlaylistClient _api = api;
	private readonly IDbSpotifyPlaylistService _playlistDb = playlistsDb;
	private readonly IDbSpotifyUserPlaylistService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyPlaylistState _state = state;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ISettingsService _settingsService = settingsService;

	private CancellationTokenSource? _cts;

	public async Task LoadAndSync(bool forceUpdate = false)
	{
		Cancel();
		_cts = new CancellationTokenSource();
		var ct = _cts.Token;

		try
		{
			var isInState = _state.Playlists is not null;
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


			await _taskManager.Run(BackgroundTaskType.Playlists, "Geting playlists", "Getting user playlists", async task =>
			{
				var userId = _spotifyApiUserService.GetUserIdRequired();

				if (!isInState)
				{
					// load data from db to state
					await task.RunStep("Loading from DB", BackgroundTaskCategory.GetDb, ct, async step =>
					{
						await LoadFromDbToState(userId, task, ct);
					});

					var shouldSync = ShouldSync(forceUpdate);
					if (!shouldSync)
					{
						// end task (synced)
						return;
					}
				}


				// load from api
				List<SpotifyPlaylist>? apiPlaylists = null;
				await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, ct, async step =>
				{
					apiPlaylists = await LoadFromApi(task, ct);
				});

				// save api data to db and state
				await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, ct, async step =>
				{
					if (apiPlaylists is null)
					{
						throw new NullReferenceException(nameof(apiPlaylists));
					}

					await SaveToDbAndState(apiPlaylists, userId, task, ct);
				});
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
		using var cts = new CancellationTokenSource();
		var ct = cts.Token;

		await _taskManager.Run(BackgroundTaskType.Playlists, "Creating playlist", $"Creating new playlists '{name}'", async task =>
		{
			var userId = _spotifyApiUserService.GetUserIdRequired();
			SpotifyPlaylist? newPlaylist = null;

			await task.RunStep("Sending API request", BackgroundTaskCategory.SaveApi, ct, async step =>
			{
				task.SetSubProgress(0.00, "api.playlist.get.start");
				var addToProfile = _settingsService.UserSettings.PlaylistAddToProfile;

				newPlaylist = await _api.CreatePlaylist(userId, name, addToProfile, ct);
				task.SetSubProgress(1.00, "api.playlist.get.done");

				task.AddLink($"Open playlist '{name}'", newPlaylist);

			});
			await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, ct, async step =>
			{
				if (newPlaylist is null)
				{
					throw new NullReferenceException(nameof(newPlaylist));
				}

				task.SetSubProgress(0.00, "db.add.playlist");
				await _playlistDb.Add(newPlaylist);

				task.SetSubProgress(0.50, "db.add.user-playlist");
				await _linkDb.AddUserPlaylist(userId, newPlaylist.Id, 0);

				_state.Add(newPlaylist);
				task.SetSubProgress(1.00, "db.add.playlist.done");
			});
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

		using var cts = new CancellationTokenSource();
		var ct = cts.Token;

		await _taskManager.Run(BackgroundTaskType.PlaylistTracks, "Adding track to playlists", $"Adding {tracksList.Count} track{trackLabel} to playlist '{playlist.Name}'", async (task) =>
		{
			string? snapshotId = null;

			await task.RunStep("Sending API request", BackgroundTaskCategory.SaveApi, ct, async step =>
			{
				task.SetSubProgress(0.00, "api.playlist-tracks.set.start");
				var trackUris = tracksList.Select(t => t.UrlApp).ToList();
				snapshotId = await _api.AddTracksToPlaylist(playlist.Id, trackUris, positionTop, ct);
				task.SetSubProgress(1.00, "api.playlist-tracks.set.done");

				task.AddLink($"Open playlist '{playlist.Name}'", playlist);
			});

			await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, ct, async step =>
			{
				if (snapshotId.IsNullOrEmpty())
				{
					throw new NullReferenceException(nameof(snapshotId));
				}

				task.SetSubProgress(0.00, "db.update.playlist.snapshot");

				var ids = tracksList.Select(t => t.Id).ToList();
				await UpdateLocalPlaylistAfterAdd(playlist, snapshotId, ids);

				// TODO playlists tracks add to db

				task.SetSubProgress(1.00, "db.update.playlist.snapshot.done");
			});
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

		using var cts = new CancellationTokenSource();
		var ct = cts.Token;

		await _taskManager.Run(BackgroundTaskType.PlaylistTracks, "Removing track from playlist", $"Removing {tracksList.Count} track{trackLabel} from playlist '{playlist.Name}'", async (task) =>
		{
			string? snapshotId = null;

			await task.RunStep("Sending API request", BackgroundTaskCategory.DeleteApi, ct, async step =>
			{
				task.SetSubProgress(0.00, "api.playlist-tracks.remove.start");
				var trackUris = tracksList.Select(t => t.UrlApp).ToList();
				snapshotId = await _api.RemoveTracksFromPlaylist(playlistId, trackUris, ct);

				task.SetSubProgress(1.00, "api.playlist-tracks.remove.done");

				task.AddLink($"Open playlist '{playlist.Name}'", playlist);
			});
			await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, ct, async step =>
			{
				if (snapshotId.IsNullOrEmpty())
				{
					throw new NullReferenceException(nameof(snapshotId));
				}

				task.SetSubProgress(0.00, "db.update.playlist.snapshot");

				var trackIds = tracksList.Select(t => t.Id).ToList();
				await UpdateLocalPlaylistAfterRemove(playlist, snapshotId, trackIds);

				// TODO playlists tracks remove from db

				task.SetSubProgress(1.00, "db.update.playlist.snapshot.done");
			});
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
