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


	private readonly Dictionary<(LoadingType Type, LoadingCategory Category), bool> _loadingStates = [];


	private void OnTaskManagerChanged()
	{
		LoadingStateChanged?.Invoke();
	}

	public void StartLoading(LoadingType type, LoadingCategory category)
	{
		SetLoading(type, category, true);
	}

	public void StopLoading(LoadingType type, LoadingCategory category)
	{
		SetLoading(type, category, false);
	}

	private void SetLoading(LoadingType type, LoadingCategory category, bool loading)
	{
		_loadingStates[(type, category)] = loading;
		LoadingStateChanged?.Invoke();
	}

	public bool IsLoading(LoadingType type, LoadingCategory category)
	{
		return _loadingStates.TryGetValue((type, category), out var value) && value;
	}
	public bool IsLoading(LoadingType type)
	{
		return _loadingStates.Where(x => x.Key.Type == type).Any(x => x.Value);
	}

}
