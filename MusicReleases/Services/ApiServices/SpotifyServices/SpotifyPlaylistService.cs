using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services.Api;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyPlaylistService(IApiUserClient spotifyUserClient, IApiPlaylistClient api, IDbSpotifyPlaylistService playlistsDb, IDbSpotifyUserPlaylistService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyPlaylistState state, IBackgroundTaskManagerService taskManager, ISettingsService settingsService, ILoadingService loadingservice) : ISpotifyPlaylistService
{
	private readonly IApiUserClient _spotifyUserClient = spotifyUserClient;
	private readonly IApiPlaylistClient _api = api;
	private readonly IDbSpotifyPlaylistService _playlistDb = playlistsDb;
	private readonly IDbSpotifyUserPlaylistService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyPlaylistState _state = state;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ISettingsService _settingsService = settingsService;
	private readonly ILoadingService _loadingservice = loadingservice;


	public async Task LoadAndSync(bool forceUpdate = false)
	{

		if (_loadingservice.IsLoading(BackgroundTaskType.PlaylistsGet))
		{
			return;
		}

		var isInState = _state.Playlists is not null;

		if (isInState)
		{
			// synced
			var shouldSync = ShouldSync(forceUpdate);

			if (!shouldSync)
			{
				// no need to sync
				return;
			}
		}

		await _taskManager.Run(BackgroundTaskType.PlaylistsGet, "Geting playlists", "Getting user playlists", async task =>
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
			var apiPlaylists = await LoadFromApi(task);

			// save api data to db and state
			await SaveToDbAndState(apiPlaylists, userId, task);
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
		return await task.StepAsync("Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			task.BeginAutoSegments(5);

			// get last sync
			var lastSync = await task.SegmentAsync("db - get playlists last sync (user-update)", async ct =>
			{
				return await _metaDb.Get(userId, SpotifyDbUpdateType.Playlists, ct);
			});

			var orderMap = await task.SegmentAsync("db - get user playlists (user-playlist)", async ct =>
			{
				return await _linkDb.GetUserPlaylistOrder(userId);
			});

			var playlistsCount = orderMap.Count;

			if (playlistsCount == 0)
			{
				await using (await task.Segment("state - set user playlists"))
				{
					_state.SetPlaylists([], lastSync);
				}

				// TODO should sync calc
				return true;
			}

			// load playlists
			var playlists = await task.SegmentAsync($"db - get playlists by ids (playlists) - {playlistsCount}", async ct =>
			{
				var playlists = await _playlistDb.GetByIds(orderMap.Keys, ct);
				return playlists.OrderBy(p => orderMap.GetValueOrDefault(p.Id, int.MaxValue)).ToList();
			});

			var shouldSync = await task.SegmentAsync($"state - set user playlists - {playlistsCount}", async ct =>
			{
				_state.SetPlaylists(playlists, lastSync);

				var shouldSync = ShouldSync(forceUpdate);

				return shouldSync;
			});

			return shouldSync;
		});
	}

	private async Task<List<SpotifyPlaylist>> LoadFromApi(BackgroundTask task)
	{
		return await task.StepAsync("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			task.BeginAutoSegments(1);

			// get saved playlists from api
			var apiPlaylists = await task.SegmentAsync($"api - get user playlists", _api.GetUserPlaylists);

			return apiPlaylists;
		});
	}

	private async Task SaveToDbAndState(List<SpotifyPlaylist> apiPlaylists, string userId, BackgroundTask task)
	{
		await task.Step("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(4);

			var apiPlaylistsCount = apiPlaylists.Count;

			// save to playlists db
			await using (await task.Segment($"db - save playlists (playlist) - {apiPlaylistsCount}"))
			{
				await _playlistDb.Save(apiPlaylists, ct);
			}

			// save to user-playlist db
			await using (await task.Segment($"db - save user playlists (user-playlist) - {apiPlaylistsCount}"))
			{
				await _linkDb.SetUserPlaylists(userId, apiPlaylists.Select(p => p.Id));
			}

			// save to update db
			await using (await task.Segment($"db - save playlists last sync (update)"))
			{
				await _metaDb.Save(userId, SpotifyDbUpdateType.Playlists, ct);
			}

			// update ui
			await using (await task.Segment($"state - set user playlists - {apiPlaylistsCount}"))
			{
				_state.SetPlaylists(apiPlaylists, DateTime.Now);
			}
		});
	}

	public async Task CreatePlaylist(string name)
	{
		await _taskManager.Run(BackgroundTaskType.PlaylistsCreate, "Creating playlist", $"Creating new playlists '{name}'", async task =>
		{
			var userId = _spotifyUserClient.GetUserIdRequired();

			var playlist = await CreatePlaylistApi(name, userId, task);

			await SavePlaylistToDbAndState(playlist, userId, task);

		});
	}

	private async Task<SpotifyPlaylist> CreatePlaylistApi(string name, string userId, BackgroundTask task)
	{
		return await task.StepAsync("Sending API request", BackgroundTaskCategory.SaveApi, async ct =>
		{
			task.BeginAutoSegments(1);

			var newPlaylist = await task.SegmentAsync("api - create playlist", async ct =>
			{
				var addToProfile = _settingsService.UserSettings.PlaylistAddToProfile;

				var newPlaylist = await _api.CreatePlaylist(userId, name, addToProfile, task.Token);

				task.AddLink("playlist", $"playlist '{name}'", newPlaylist);
				return newPlaylist;
			});

			return newPlaylist;
		});
	}

	private async Task SavePlaylistToDbAndState(SpotifyPlaylist playlist, string userId, BackgroundTask task)
	{
		await task.Step("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(3);

			// save to playlist db
			await using (await task.Segment("db - add playlist (playlist)"))
			{
				await _playlistDb.Add(playlist, ct);
			}

			// save to user-playlist db
			await using (await task.Segment("db - add to user playlist (user-playlist)"))
			{
				await _linkDb.AddUserPlaylist(userId, playlist.Id, 0);
			}

			// update ui
			await using (await task.Segment("state - add user playlist"))
			{
				_state.Add(playlist);
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

		var trackLabel = tracksList.Count == 1 ? string.Empty : "s";

		var playlist
			= _state.GetById(playlistId)
			?? throw new InvalidOperationException($"Playlist not found in state");

		await _taskManager.Run(BackgroundTaskType.PlaylistTracksAdd, "Adding track to playlists", $"Adding {tracksList.Count} track{trackLabel} to playlist '{playlist.Name}'", async task =>
		{
			var snapshotId = await AddTracksApi(playlist, tracksList, positionTop, task);

			await SaveNewTracksToDbAndStore(playlist, snapshotId, tracksList, task);
		});
	}

	private async Task<string> AddTracksApi(SpotifyPlaylist playlist, IEnumerable<SpotifyTrack> tracks, bool positionTop, BackgroundTask task)
	{
		return await task.StepAsync("Sending API request", BackgroundTaskCategory.SaveApi, async step =>
		{
			task.BeginAutoSegments(1);

			var tracksCount = tracks.Count();

			return await task.SegmentAsync($"api - add playlist tracks - {tracksCount}", async ct =>
			{
				var trackUris = tracks.Select(t => t.UrlApp).ToList();

				var snapshotId = await _api.AddTracksToPlaylist(playlist.Id, trackUris, positionTop, ct);

				task.AddLink("playlist", $"playlist '{playlist.Name}'", playlist);

				return snapshotId;
			});
		});
	}

	private async Task SaveNewTracksToDbAndStore(SpotifyPlaylist playlist, string snapshotId, List<SpotifyTrack> tracks, BackgroundTask task)
	{
		await task.Step("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(2);

			// update snapshot in db
			await using (await task.Segment("db - update playlist snapshot (playlist)"))
			{
				await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId, ct);
			}
			// TODO save tracks to db

			// update state
			await using (await task.Segment("state - update playlist snapshot and tracks"))
			{
				var trackIds = tracks.Select(t => t.Id).ToList();
				await UpdateStateAfterAdding(playlist, snapshotId, trackIds);
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

		await _taskManager.Run(BackgroundTaskType.PlaylistTracksRemove, "Removing track from playlist", $"Removing {tracksList.Count} track{trackLabel} from playlist '{playlist.Name}'", async (task) =>
		{
			var snapshotId = await RemoveTracksApi(playlist, tracks, task);

			await SaveRemovedTracksToDbAndStore(playlist, snapshotId, tracks, task);
		});
	}


	private async Task<string> RemoveTracksApi(SpotifyPlaylist playlist, IEnumerable<SpotifyTrack> tracks, BackgroundTask task)
	{
		return await task.StepAsync("Sending API request", BackgroundTaskCategory.DeleteApi, async ct =>
		{
			task.BeginAutoSegments(1);

			var tracksCount = tracks.Count();

			return await task.SegmentAsync($"api - add playlist tracks - {tracksCount}", async ct =>
			{
				var trackUris = tracks.Select(t => t.UrlApp).ToList();

				var snapshotId = await _api.RemoveTracksFromPlaylist(playlist.Id, trackUris, ct);

				task.AddLink("playlist", $"playlist '{playlist.Name}'", playlist);

				return snapshotId;
			});
		});
	}

	private async Task SaveRemovedTracksToDbAndStore(SpotifyPlaylist playlist, string snapshotId, IEnumerable<SpotifyTrack> tracks, BackgroundTask task)
	{
		await task.Step("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(2);

			// update snapshot in db
			await using (await task.Segment("db - update playlist snapshot (playlist)"))
			{
				await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId, ct);
			}
			// TODO save tracks to db

			// update state
			await using (await task.Segment("state - update playlist snapshot and tracks"))
			{
				var trackIds = tracks.Select(t => t.Id).ToList();
				await UpdateStateAfterRemoving(playlist, snapshotId, trackIds);
			}
		});
	}

	private async Task UpdateStateAfterAdding(SpotifyPlaylist playlist, string snapshotId, List<string> trackIds)
	{
		// update state
		playlist.SnapshotId = snapshotId;

		foreach (var trackId in trackIds)
		{
			if (playlist.Tracks is ICollection<string> tracks)
			{
				tracks.Add(trackId);
			}
		}
	}

	private async Task UpdateStateAfterRemoving(SpotifyPlaylist playlist, string snapshotId, List<string> trackIds)
	{
		// update state
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
