using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Playlists;
using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

internal sealed class SpotifyWorkflowService
(
	IBackgroundTaskManagerService taskManager,
	ISpotifyArtistService artistService,
	ISpotifyReleaseService releaseService,
	ISpotifyPlaylistService playlistService
)
: ISpotifyWorkflowService
{
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ISpotifyArtistService _artistService = artistService;
	private readonly ISpotifyReleaseService _releaseService = releaseService;
	private readonly ISpotifyPlaylistService _playlistService = playlistService;


	public Task StartLoadingAll(ReleaseGroup releaseType, bool forceUpdate)
	{
		_taskManager.StartWorkflow();

		EnqueueChain(true, true, true, releaseType, forceUpdate);

		return Task.CompletedTask;
	}

	public Task Update(UpdateButtonComponent updateType, ReleaseGroup releaseType)
	{
		_taskManager.StartWorkflow();

		switch (updateType)
		{
			case UpdateButtonComponent.Artists:
				EnqueueChain(true, false, false, releaseType, true);
				break;

			case UpdateButtonComponent.Releases:
				EnqueueChain(true, true, false, releaseType, true);
				break;

			case UpdateButtonComponent.Playlists:
				EnqueueChain(true, true, true, releaseType, true);
				break;

			default:
				throw new NotSupportedException(nameof(updateType));
		}

		return Task.CompletedTask;
	}

	private void EnqueueChain(bool includeArtists, bool includeReleases, bool includePlaylists, ReleaseGroup releaseType, bool forceUpdate)
	{
		if (includeArtists)
		{
			var artistsRequest = new BackgroundTaskRequest
			(
				BackgroundTaskType.ArtistsGet,
				"Artists",
				"DB → API → DB",
				3,
				task => _artistService.GetInTask(task, forceUpdate),
				null
			);
			_taskManager.Enqueue(artistsRequest);
		}

		if (includeReleases)
		{
			var releasesRequest = new BackgroundTaskRequest
			(
				BackgroundTaskType.ReleasesGet,
				"Releases",
				"DB → API → DB",
				3,
				task => _releaseService.GetInTask(task, releaseType, forceUpdate),
				includeArtists
					? [BackgroundTaskType.ArtistsGet]
					: null
			);

			_taskManager.Enqueue(releasesRequest);
		}

		if (includePlaylists)
		{
			var playlistsRequest = new BackgroundTaskRequest
			(
				BackgroundTaskType.PlaylistsGet,
				"Playlists",
				"DB → API → DB",
				3,
				task => _playlistService.GetInTask(task, forceUpdate),
				[BackgroundTaskType.ReleasesGet]
			);

			_taskManager.Enqueue(playlistsRequest);
		}
	}

}