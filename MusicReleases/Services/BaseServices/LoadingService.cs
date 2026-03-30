using JakubKastner.MusicReleases.Spotify.Tasks;

namespace JakubKastner.MusicReleases.Services.BaseServices;

internal class LoadingService : IDisposable, ILoadingService
{
	private readonly IBackgroundTaskManagerService _backgroundTaskManagerService;

	public LoadingService(IBackgroundTaskManagerService spotifyTaskManagerService)
	{
		_backgroundTaskManagerService = spotifyTaskManagerService;
		_backgroundTaskManagerService.OnUiRelevantChange += OnTaskManagerChanged;
	}

	public void Dispose()
	{
		_backgroundTaskManagerService.OnUiRelevantChange -= OnTaskManagerChanged;
		GC.SuppressFinalize(this);
	}

	public event Action? LoadingStateChanged;

	public bool Loading => _backgroundTaskManagerService.AnyTaskRunning;

	public string ActiveClass => Loading.ToCssClass("active");


	private void OnTaskManagerChanged()
	{
		LoadingStateChanged?.Invoke();
	}

	public bool IsLoading(BackgroundTaskType type)
	{
		return _backgroundTaskManagerService.RunningTasks.Any(x => x.Type == type);
	}

}
