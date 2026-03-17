using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Extensions;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Base;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal sealed class SpotifyPlaylistService(ISpotifyUserClient userApi, ISpotifyPlaylistClient playlistApi, ISpotifyPlaylistDbService playlistsDb, ISpotifyUserPlaylistDbService userPlaylistDb, IDbSpotifyUserUpdateService updateDb, ISpotifyPlaylistState playlistState, IBackgroundTaskManagerService taskManager, ISettingsService settingsService, ILoadingService loadingService) : SpotifyBaseSyncService<SpotifyPlaylist, SpotifyPlaylistEntity, SpotifyUserPlaylistEntity, SpotifyUserPlaylistPayload>(userApi, playlistsDb, userPlaylistDb, updateDb, playlistState, taskManager, loadingService), ISpotifyPlaylistService
{
	private readonly ISpotifyUserClient _userApi = userApi;
	private readonly ISpotifyPlaylistClient _playlistApi = playlistApi;
	private readonly ISpotifyPlaylistDbService _playlistDb = playlistsDb;
	private readonly ISpotifyUserPlaylistDbService _userPlaylistDb = userPlaylistDb;
	private readonly IDbSpotifyUserUpdateService _updateDb = updateDb;
	private readonly ISpotifyPlaylistState _playlistState = playlistState;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ISettingsService _settingsService = settingsService;


	protected override BackgroundTaskType TaskType => BackgroundTaskType.PlaylistsGet;

	protected override SpotifyDbUpdateType DbUpdateType => SpotifyDbUpdateType.Playlists;
	protected override string TaskTitle => "Geting playlists";
	protected override string TaskDescription => "Getting user playlists";
	protected override string EntityName => "playlists";
	protected override string UserLinkLabel => "user-playlist";
	protected override DateTime? LastSync => _playlistState.LastSync;
	protected override bool IsDataInState => _playlistState.Items is not null;

	protected override async Task<IReadOnlyCollection<SpotifyPlaylist>> ApiLoad(CancellationToken ct) => await _playlistApi.GetUserPlaylists(ct);

	protected override SpotifyUserPlaylistPayload CreatePayload(SpotifyPlaylist model) => model.ToPayload();

	public async Task CreatePlaylist(string name)
	{
		await _taskManager.Run(BackgroundTaskType.PlaylistsCreate, "Creating playlist", $"Creating new playlists '{name}'", async task =>
		{
			var userId = _userApi.GetUserIdRequired();

			var playlist = await CreatePlaylistApi(name, userId, task);

			await SavePlaylistToDbAndState(playlist, userId, task);
		});
	}

	private async Task<SpotifyPlaylist> CreatePlaylistApi(string name, string userId, BackgroundTask task)
	{
		return await task.RunStep("Sending API request", BackgroundTaskCategory.SaveApi, async ct =>
		{
			task.BeginAutoSegments(1);

			var newPlaylist = await task.RunSegment("api - create playlist", async ct =>
			{
				var addToProfile = _settingsService.UserSettings.PlaylistAddToProfile;
				var lastPlaylistOrder = _playlistState.Items?.Max(p => p.Order) ?? 0;
				var order = lastPlaylistOrder + 1;

				var newPlaylist = await _playlistApi.CreatePlaylist(userId, name, addToProfile, order, task.Ct);

				task.AddLink("playlist", $"playlist '{name}'", newPlaylist);
				return newPlaylist;
			});

			return newPlaylist;
		});
	}

	private async Task SavePlaylistToDbAndState(SpotifyPlaylist playlist, string userId, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(3);

			// save to playlist db
			await task.RunSegment("db - add playlist (playlist)", async ct =>
			{
				await _playlistDb.Save(playlist, ct);
			});

			// save to user-playlist db
			await task.RunSegment("db - add to user playlist (user-playlist)", async ct =>
			{
				await _userPlaylistDb.AddNew(playlist, userId, ct);
			});

			// update ui
			await task.RunSegment("state - add user playlist", async ct =>
			{
				_playlistState.Add(playlist);
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

		var trackLabel = tracksList.Count == 1 ? string.Empty : "s";

		var playlist
			= _playlistState.GetById(playlistId)
			?? throw new InvalidOperationException($"Playlist not found in state");

		await _taskManager.Run(BackgroundTaskType.PlaylistTracksAdd, "Adding track to playlists", $"Adding {tracksList.Count} track{trackLabel} to playlist '{playlist.Name}'", async task =>
		{
			var snapshotId = await AddTracksApi(playlist, tracksList, positionTop, task);

			await SaveNewTracksToDbAndStore(playlist, snapshotId, tracksList, task);
		});
	}

	private async Task<string> AddTracksApi(SpotifyPlaylist playlist, IEnumerable<SpotifyTrack> tracks, bool positionTop, BackgroundTask task)
	{
		return await task.RunStep("Sending API request", BackgroundTaskCategory.SaveApi, async step =>
		{
			task.BeginAutoSegments(1);

			var tracksCount = tracks.Count();

			return await task.RunSegment($"api - add playlist tracks - {tracksCount}", async ct =>
			{
				var trackUris = tracks.Select(t => t.UrlApp).ToList();

				var snapshotId = await _playlistApi.AddTracksToPlaylist(playlist.Id, trackUris, positionTop, ct);

				task.AddLink("playlist", $"playlist '{playlist.Name}'", playlist);

				return snapshotId;
			});
		});
	}

	private async Task SaveNewTracksToDbAndStore(SpotifyPlaylist playlist, string snapshotId, List<SpotifyTrack> tracks, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(2);

			// update snapshot in db
			await task.RunSegment("db - update playlist snapshot (playlist)", async ct =>
			{
				await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId, ct);
			});
			// TODO save tracks to db

			// update state
			await task.RunSegment("state - update playlist snapshot and tracks", async ct =>
			{
				var trackIds = tracks.Select(t => t.Id).ToList();
				await UpdateStateAfterAdding(playlist, snapshotId, trackIds);
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

		var playlist = _playlistState.GetById(playlistId) ?? throw new InvalidOperationException("Playlist not found in state");

		await _taskManager.Run(BackgroundTaskType.PlaylistTracksRemove, "Removing track from playlist", $"Removing {tracksList.Count} track{trackLabel} from playlist '{playlist.Name}'", async (task) =>
		{
			var snapshotId = await RemoveTracksApi(playlist, tracks, task);

			await SaveRemovedTracksToDbAndStore(playlist, snapshotId, tracks, task);
		});
	}


	private async Task<string> RemoveTracksApi(SpotifyPlaylist playlist, IEnumerable<SpotifyTrack> tracks, BackgroundTask task)
	{
		return await task.RunStep("Sending API request", BackgroundTaskCategory.DeleteApi, async ct =>
		{
			task.BeginAutoSegments(1);

			var tracksCount = tracks.Count();

			return await task.RunSegment($"api - add playlist tracks - {tracksCount}", async ct =>
			{
				var trackUris = tracks.Select(t => t.UrlApp).ToList();

				var snapshotId = await _playlistApi.RemoveTracksFromPlaylist(playlist.Id, trackUris, ct);

				task.AddLink("playlist", $"playlist '{playlist.Name}'", playlist);

				return snapshotId;
			});
		});
	}

	private async Task SaveRemovedTracksToDbAndStore(SpotifyPlaylist playlist, string snapshotId, IEnumerable<SpotifyTrack> tracks, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(2);

			// update snapshot in db
			await task.RunSegment("db - update playlist snapshot (playlist)", async ct =>
			{
				await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId, ct);
			});
			// TODO save tracks to db

			// update state
			await task.RunSegment("state - update playlist snapshot and tracks", async ct =>
			{
				var trackIds = tracks.Select(t => t.Id).ToList();
				await UpdateStateAfterRemoving(playlist, snapshotId, trackIds);
			});
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
