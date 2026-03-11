using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.SpotifyServices;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class LoadingService : IDisposable, ILoadingService
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
