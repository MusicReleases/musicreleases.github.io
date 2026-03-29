using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Playlists;
using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

internal sealed class SpotifyWorkflowService(
	IBackgroundTaskManagerService taskManager,
	ISpotifyArtistService artistService,
	ISpotifyReleaseService releaseService,
	ISpotifyPlaylistService playlistService)
	: ISpotifyWorkflowService
{
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ISpotifyArtistService _artistService = artistService;
	private readonly ISpotifyReleaseService _releaseService = releaseService;
	private readonly ISpotifyPlaylistService _playlistService = playlistService;

	public Task StartLoadingAll(ReleaseGroup releaseType, bool forceUpdate)
	{
		_taskManager.StartWorkflow();
		EnqueueChain(includeArtists: true, includeReleases: true, includePlaylists: true, releaseType, forceUpdate);
		return Task.CompletedTask;
	}

	public Task Update(UpdateButtonComponent updateType, ReleaseGroup releaseType)
	{
		_taskManager.StartWorkflow();

		switch (updateType)
		{
			case UpdateButtonComponent.Artists:
				{
					EnqueueChain(true, false, false, releaseType, true);
					break;
				}
			case UpdateButtonComponent.Releases:
				{
					EnqueueChain(true, true, false, releaseType, true);
					break;
				}
			case UpdateButtonComponent.Playlists:
				{
					EnqueueChain(true, true, true, releaseType, true);
					break;
				}
			default:
				{
					throw new NotSupportedException(nameof(updateType));
				}
		}

		return Task.CompletedTask;
	}

	private void EnqueueChain(bool includeArtists, bool includeReleases, bool includePlaylists, ReleaseGroup releaseType, bool forceUpdate)
	{
		var artistsSteps = NewSteps(includeArtists);
		var releasesSteps = NewSteps(includeReleases);
		var playlistsSteps = NewSteps(includePlaylists);


		Guid? dbBarrier
			= includePlaylists && playlistsSteps.HasValue
			? playlistsSteps.Value.Db
			: includeReleases && releasesSteps.HasValue ? releasesSteps.Value.Db : null;

		var artistsPlan = includeArtists && artistsSteps.HasValue
			? new BackgroundTaskSyncPlan(
				DbStepId: artistsSteps.Value.Db,
				ApiStepId: artistsSteps.Value.Api,
				SaveStepId: artistsSteps.Value.Save,
				WaitBeforeDbStepId: null,
				WaitBeforeApiStepId: dbBarrier)
			: null;

		var releasesPlan = includeReleases && releasesSteps.HasValue
			? new BackgroundTaskSyncPlan(
				DbStepId: releasesSteps.Value.Db,
				ApiStepId: releasesSteps.Value.Api,
				SaveStepId: releasesSteps.Value.Save,
				WaitBeforeDbStepId: includeArtists && artistsSteps.HasValue ? artistsSteps.Value.Db : null,
				WaitBeforeApiStepId: includeArtists && artistsSteps.HasValue ? artistsSteps.Value.Save : null)
			: null;

		var playlistsPlan = includePlaylists && playlistsSteps.HasValue
			? new BackgroundTaskSyncPlan(
				DbStepId: playlistsSteps.Value.Db,
				ApiStepId: playlistsSteps.Value.Api,
				SaveStepId: playlistsSteps.Value.Save,
				WaitBeforeDbStepId: includeReleases && releasesSteps.HasValue ? releasesSteps.Value.Db : null,
				WaitBeforeApiStepId: includeReleases && releasesSteps.HasValue ? releasesSteps.Value.Save : null)
			: null;

		if (includeArtists && artistsPlan is not null)
		{
			_taskManager.Enqueue(new(
				Type: BackgroundTaskType.ArtistsGet,
				Name: "Artists",
				Info: "DB → API → DB",
				ExpectedSteps: 3,
				Work: t => _artistService.GetInTask(t, artistsPlan, forceUpdate),
				DependsOn: []
			));
		}

		if (includeReleases && releasesPlan is not null)
		{
			_taskManager.Enqueue(new(
				Type: BackgroundTaskType.ReleasesGet,
				Name: "Releases",
				Info: "DB → API → DB",
				ExpectedSteps: 3,
				Work: t => _releaseService.GetInTask(t, releasesPlan, releaseType, forceUpdate),
				DependsOn: []
			));
		}

		if (includePlaylists && playlistsPlan is not null)
		{
			_taskManager.Enqueue(new(
				Type: BackgroundTaskType.PlaylistsGet,
				Name: "Playlists",
				Info: "DB → API → DB",
				ExpectedSteps: 3,
				Work: t => _playlistService.GetInTask(t, playlistsPlan, forceUpdate),
				DependsOn: []
			));
		}
	}

	private static Steps? NewSteps(bool create)
	{
		if (!create)
		{
			return null;
		}

		return new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
	}

	private readonly record struct Steps(Guid Db, Guid Api, Guid Save);
}