using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Playlists;
using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

internal sealed class SpotifyWorkflowService(IBackgroundTaskManagerService taskManager, ISpotifyArtistService artistService, ISpotifyReleaseService releaseService, ISpotifyPlaylistService playlistService) : ISpotifyWorkflowService
{
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ISpotifyArtistService _artistService = artistService;
	private readonly ISpotifyReleaseService _releaseService = releaseService;
	private readonly ISpotifyPlaylistService _playlistService = playlistService;

	public Task StartLoadingAll(ReleaseGroup releaseType, bool forceUpdate)
	{
		_taskManager.StartWorkflow();

		EnqueueArtistsWithReleases(releaseType, forceUpdate);
		EnqueuePlaylists(forceUpdate);

		return Task.CompletedTask;
	}

	public Task Update(UpdateButtonComponent updateType, ReleaseGroup releaseType)
	{
		_taskManager.StartWorkflow();

		switch (updateType)
		{
			case UpdateButtonComponent.Artists:
				EnqueueArtistsWithReleases(releaseType, true);
				break;

			case UpdateButtonComponent.Releases:
				EnqueueReleases(releaseType, true);
				break;

			case UpdateButtonComponent.Playlists:
				EnqueuePlaylists(true);
				break;

			default:
				throw new NotSupportedException(nameof(updateType));
		}

		return Task.CompletedTask;
	}

	private void EnqueueArtistsWithReleases(ReleaseGroup releaseType, bool forceUpdate)
	{
		_taskManager.Enqueue(new(
			Type: BackgroundTaskType.ArtistsGet,
			Name: "Artists",
			Info: "Loading followed artists",
			ExpectedSteps: 3,
			Work: t => _artistService.Get(forceUpdate),
			DependsOn: []
		));

		EnqueueReleases(releaseType, forceUpdate, [BackgroundTaskType.ArtistsGet]);
	}

	private void EnqueueReleases(ReleaseGroup releaseType, bool forceUpdate, IReadOnlyCollection<BackgroundTaskType>? dependsOn = null)
	{
		_taskManager.Enqueue(new(
			Type: BackgroundTaskType.ReleasesGet,
			Name: "Releases",
			Info: "Updating releases",
			ExpectedSteps: 3,
			Work: t => _releaseService.Get(releaseType, forceUpdate),
			DependsOn: dependsOn ?? []
		));
	}

	private void EnqueuePlaylists(bool forceUpdate)
	{
		_taskManager.Enqueue(new(
			Type: BackgroundTaskType.PlaylistsGet,
			Name: "Playlists",
			Info: "Loading playlists",
			ExpectedSteps: 3,
			Work: t => _playlistService.Get(forceUpdate),
			DependsOn: []
		));

		// TODO playlist tracks
	}
}