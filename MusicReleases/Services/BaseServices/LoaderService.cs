using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.SpotifyServices;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class LoaderService : ILoaderService, IDisposable
{
	private readonly ISpotifyTaskManagerService _spotifyTaskManagerService;

	public LoaderService(ISpotifyTaskManagerService spotifyTaskManagerService)
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

	public bool Loading => _loadingStates.Values.Any(x => x) || _spotifyTaskManagerService.IsAnyTaskRunning;

	public string ActiveClass => Loading.ToCssClass("active");


	private readonly Dictionary<(BackgroundTaskType Type, BackgroundTaskCategory Category), bool> _loadingStates = [];


	private void OnTaskManagerChanged()
	{
		LoadingStateChanged?.Invoke();
	}

	public void StartLoading(BackgroundTaskType type, BackgroundTaskCategory category)
	{
		SetLoading(type, category, true);
	}

	public void StopLoading(BackgroundTaskType type, BackgroundTaskCategory category)
	{
		SetLoading(type, category, false);
	}

	private void SetLoading(BackgroundTaskType type, BackgroundTaskCategory category, bool loading)
	{
		_loadingStates[(type, category)] = loading;
		LoadingStateChanged?.Invoke();
	}

	public bool IsLoading(BackgroundTaskType type, BackgroundTaskCategory category)
	{
		return _loadingStates.TryGetValue((type, category), out var value) && value;
	}
	public bool IsLoading(BackgroundTaskType type)
	{
		return _loadingStates.Where(x => x.Key.Type == type).Any(x => x.Value);
	}

}
