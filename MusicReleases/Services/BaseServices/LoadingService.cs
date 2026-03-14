using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Services;

namespace JakubKastner.MusicReleases.Services.BaseServices;

internal class LoadingService : IDisposable, ILoadingService
{
	private readonly IBackgroundTaskManagerService _spotifyTaskManagerService;

	public LoadingService(IBackgroundTaskManagerService spotifyTaskManagerService)
	{
		_spotifyTaskManagerService = spotifyTaskManagerService;
		_spotifyTaskManagerService.OnChange += OnTaskManagerChanged;
	}

	public void Dispose()
	{
		_spotifyTaskManagerService.OnChange -= OnTaskManagerChanged;
		GC.SuppressFinalize(this);
	}

	public event Action? LoadingStateChanged;

	public bool Loading => _spotifyTaskManagerService.IsAnyTaskRunning;

	public string ActiveClass => Loading.ToCssClass("active");


	private void OnTaskManagerChanged()
	{
		LoadingStateChanged?.Invoke();
	}

	public bool IsLoading(BackgroundTaskType type)
	{
		return _spotifyTaskManagerService.RunningTasks.Any(x => x.Type == type);
	}

}
