using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class LoaderService : ILoaderService
{
	private readonly ISpotifyTaskManagerService _spotifyTaskManagerService;


	private readonly Dictionary<(Enums.LoadingType Type, Enums.LoadingCategory Category), bool> _loadingStates = [];

	public LoaderService(ISpotifyTaskManagerService spotifyTaskManagerService)
	{
		_spotifyTaskManagerService = spotifyTaskManagerService;
		_spotifyTaskManagerService.OnChange += OnTaskManagerChanged;
	}


	public event Action? LoadingStateChanged;

	public bool Loading => _loadingStates.Values.Any(x => x) || _spotifyTaskManagerService.IsAnyTaskRunning;
	public string LoadingClass => Loading ? "active" : string.Empty;

	private void OnTaskManagerChanged()
	{
		LoadingStateChanged?.Invoke();
	}

	public void Dispose()
	{
		_spotifyTaskManagerService.OnChange -= OnTaskManagerChanged;
	}

	public void StartLoading(Enums.LoadingType type, Enums.LoadingCategory category)
	{
		SetLoading(type, category, true);
	}

	public void StopLoading(Enums.LoadingType type, Enums.LoadingCategory category)
	{
		SetLoading(type, category, false);
	}

	private void SetLoading(Enums.LoadingType type, Enums.LoadingCategory category, bool loading)
	{
		_loadingStates[(type, category)] = loading;
		LoadingStateChanged?.Invoke();
	}

	public bool IsLoading(Enums.LoadingType type, Enums.LoadingCategory category)
	{
		return _loadingStates.TryGetValue((type, category), out var value) && value;
	}
	public bool IsLoading(Enums.LoadingType type)
	{
		return _loadingStates.Where(x => x.Key.Type == type).Any(x => x.Value);
	}

}
