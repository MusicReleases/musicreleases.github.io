using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Base;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.MusicReleases.Spotify.Settings;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Playlists;
using JakubKastner.SpotifyApi.Tracks;
using JakubKastner.SpotifyApi.User;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal sealed class SpotifyPlaylistService
(
	ISpotifyUserClient userApi,
	ISpotifyPlaylistClient playlistApi,
	ISpotifyPlaylistDbService playlistDb,
	ISpotifyReadByPayloadService<SpotifyPlaylist, SpotifyUserPlaylistPayload> playlistReader,
	ISpotifyWriteEntityService<SpotifyPlaylist> playlistWriter,
	ISpotifyUserPlaylistDbService userPlaylistDb,
	ISpotifyUserUpdateDbService updateDb,
	ISpotifyPlaylistState playlistState,
	IBackgroundTaskManagerService taskManager,
	ILoadingService loadingService,
	ISpotifySettingsService settingsService
)
: SpotifyBaseSyncService<SpotifyPlaylist, SpotifyUserPlaylistPayload>(userApi, playlistReader, playlistWriter, userPlaylistDb, updateDb, playlistState, taskManager, loadingService), ISpotifyPlaylistService
{
	private readonly ISpotifyUserClient _userApi = userApi;
	private readonly ISpotifyPlaylistClient _playlistApi = playlistApi;
	private readonly ISpotifyPlaylistDbService _playlistDb = playlistDb;
	private readonly ISpotifyWriteEntityService<SpotifyPlaylist> _playlistWriter = playlistWriter;
	private readonly ISpotifyUserPlaylistDbService _userPlaylistDb = userPlaylistDb;
	private readonly ISpotifyPlaylistState _playlistState = playlistState;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ISpotifySettingsService _settingsService = settingsService;

	protected override BackgroundTaskType TaskType => BackgroundTaskType.PlaylistsGet;

	protected override SpotifyDbUpdateType DbUpdateType => SpotifyDbUpdateType.Playlists;
	protected override string TaskTitle => "Geting playlists";
	protected override string TaskDescription => "Getting user playlists";
	protected override string EntityName => "playlists";
	protected override string UserLinkLabel => "user-playlist";
	protected override DateTime? LastSync => _playlistState.LastSync;
	protected override bool IsDataInState => _playlistState.Items is not null;

	protected override IAsyncEnumerable<IReadOnlyCollection<SpotifyPlaylist>> ApiLoadBatches(CancellationToken ct) => _playlistApi.GetUserPlaylistsBatches(25, ct);

	protected override SpotifyUserPlaylistPayload CreatePayload(SpotifyPlaylist model) => model.ToPayload();

	public Task GetInTask(BackgroundTask task, bool forceUpdate = false) => RunGetInExistingTask(default, forceUpdate, task, null);

	public async Task CreatePlaylist(string name)
	{
		await _taskManager.Run
		(
			BackgroundTaskType.PlaylistsCreate,
			"Creating playlist",
			$"Creating new playlists '{name}'",
			2,
			async task =>
			{
				var userId = _userApi.GetUserIdRequired();

				var playlist = await CreatePlaylistApi(name, userId, task);

				await SavePlaylistToDbAndState(playlist, userId, task);
			}
		);
	}

	private async Task<SpotifyPlaylist> CreatePlaylistApi(string name, string userId, BackgroundTask task)
	{
		return await task.RunStep
		(
			BackgroundTaskCategory.SaveApi,
			async (ct, step) =>
			{
				//step.BeginAutoSegments(1);

				return await step.RunSegment("api - create playlist", async ct2 =>
				{
					var addToProfile = _settingsService.UserSettings.PlaylistAddToProfile;
					var lastPlaylistOrder = _playlistState.Items?.Max(p => p.Order) ?? 0;
					var order = lastPlaylistOrder + 1;

					var newPlaylist = await _playlistApi.CreatePlaylist(userId, name, addToProfile, order, ct2);

					task.AddLink("playlist", $"playlist '{name}'", newPlaylist);

					return newPlaylist;
				});
			}
		);
	}

	private async Task SavePlaylistToDbAndState(SpotifyPlaylist playlist, string userId, BackgroundTask task)
	{
		await task.RunStep
		(
			BackgroundTaskCategory.SaveDb,
			async (ct, step) =>
			{
				//step.BeginAutoSegments(3);

				// save to playlist db
				await step.RunSegment("db - add playlist (playlist)", async ct2 =>
				{
					await _playlistWriter.Save(playlist, true, ct2);
				});

				// save to user-playlist db
				await step.RunSegment("db - add to user playlist (user-playlist)", async ct2 =>
				{
					await _userPlaylistDb.AddNew(playlist, userId, ct2);
				});

				// update ui
				await step.RunSegment("state - add user playlist", async ct2 =>
				{
					_playlistState.Add(playlist);
				});
			}
		);
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

		await _taskManager.Run
		(
			BackgroundTaskType.PlaylistTracksAdd,
			"Adding tracks to playlist",
			$"Adding {tracksList.Count} track{trackLabel} to playlist '{playlist.Name}'",
			2,
			async task =>
			{
				var snapshotId = await AddTracksApi(playlist, tracksList, positionTop, task);

				await SaveNewTracksToDbAndStore(playlist, snapshotId, tracksList, task);
			}
		);
	}

	private async Task<string> AddTracksApi(SpotifyPlaylist playlist, IEnumerable<SpotifyTrack> tracks, bool positionTop, BackgroundTask task)
	{
		return await task.RunStep
		(
			BackgroundTaskCategory.SaveApi,
			async (ct, step) =>
			{
				//step.BeginAutoSegments(1);

				var tracksCount = tracks.Count();

				return await step.RunSegment($"api - add playlist tracks - {tracksCount}", async ct2 =>
				{
					var trackUris = tracks.Select(t => t.UrlApp).ToList();

					var snapshotId = await _playlistApi.AddTracksToPlaylist(playlist.Id, trackUris, positionTop, ct2);

					task.AddLink("playlist", $"playlist '{playlist.Name}'", playlist);

					return snapshotId;
				});
			}
		);
	}

	private async Task SaveNewTracksToDbAndStore(SpotifyPlaylist playlist, string snapshotId, List<SpotifyTrack> tracks, BackgroundTask task)
	{
		await task.RunStep
		(
			BackgroundTaskCategory.SaveDb,
			async (ct, step) =>
			{
				//step.BeginAutoSegments(2);

				// update snapshot in db
				await step.RunSegment("db - update playlist snapshot (playlist)", async ct2 =>
				{
					await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId, ct2);
				});
				// TODO db tracks - save

				// update state
				await step.RunSegment("state - update playlist snapshot and tracks", async ct2 =>
				{
					var trackIds = tracks.Select(t => t.Id).ToList();
					await UpdateStateAfterAdding(playlist, snapshotId, trackIds);
				});
			}
		);
	}

	public async Task RemoveTracks(string playlistId, IEnumerable<SpotifyTrack> tracks)
	{
		var tracksList = tracks.ToList();
		if (tracksList.Count == 0)
		{
			return;
		}
		var trackLabel = tracksList.Count == 1 ? "" : "s";

		var playlist
			= _playlistState.GetById(playlistId)
			?? throw new InvalidOperationException("Playlist not found in state");

		await _taskManager.Run
		(
			BackgroundTaskType.PlaylistTracksAdd,
			 "Removing tracks from playlist",
			$"Removing {tracksList.Count} track{trackLabel} from playlist '{playlist.Name}'",
			2,
			async task =>
			{
				var snapshotId = await RemoveTracksApi(playlist, tracks, task);

				await SaveRemovedTracksToDbAndStore(playlist, snapshotId, tracks, task);
			}
		);
	}

	private async Task<string> RemoveTracksApi(SpotifyPlaylist playlist, IEnumerable<SpotifyTrack> tracks, BackgroundTask task)
	{
		return await task.RunStep
		(
			BackgroundTaskCategory.DeleteApi,
			async (ct, step) =>
			{
				//step.BeginAutoSegments(1);

				var tracksCount = tracks.Count();

				return await step.RunSegment($"api - add playlist tracks - {tracksCount}", async ct2 =>
				{
					var trackUris = tracks.Select(t => t.UrlApp).ToList();

					var snapshotId = await _playlistApi.RemoveTracksFromPlaylist(playlist.Id, trackUris, ct2);

					task.AddLink("playlist", $"playlist '{playlist.Name}'", playlist);

					return snapshotId;
				});
			}
		);
	}

	private async Task SaveRemovedTracksToDbAndStore(SpotifyPlaylist playlist, string snapshotId, IEnumerable<SpotifyTrack> tracks, BackgroundTask task)
	{
		await task.RunStep
		(
			BackgroundTaskCategory.SaveDb,
			async (ct, step) =>
			{
				//step.BeginAutoSegments(2);

				// update snapshot in db
				await step.RunSegment("db - update playlist snapshot (playlist)", async ct2 =>
				{
					await _playlistDb.UpdateSnapshot(playlist.Id, snapshotId, ct2);
				});
				// TODO db tracks - save

				// update state
				await step.RunSegment("state - update playlist snapshot and tracks", async ct2 =>
				{
					var trackIds = tracks.Select(t => t.Id).ToList();
					await UpdateStateAfterRemoving(playlist, snapshotId, trackIds);
				});
			}
		);
	}

	private static async Task UpdateStateAfterAdding(SpotifyPlaylist playlist, string snapshotId, List<string> trackIds)
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

	private static async Task UpdateStateAfterRemoving(SpotifyPlaylist playlist, string snapshotId, List<string> trackIds)
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
