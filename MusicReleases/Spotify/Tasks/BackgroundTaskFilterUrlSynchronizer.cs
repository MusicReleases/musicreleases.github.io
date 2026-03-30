using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.Spotify.Tasks.User;

namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class BackgroundTaskFilterUrlSynchronizer : IBackgroundTaskFilterUrlSynchronizer
{
	private readonly IBackgroundTaskFilterService _filterService;
	private readonly IBackgroundTaskFilterUrlService _filterUrlService;
	private readonly ISpotifyUserTaskFilterDbService _dbService;
	private readonly INavigationService _navService;

	private const string _baseUrl = "/tasks";

	// feedback loop guard
	private bool _suppressUrlUpdate;

	public BackgroundTaskFilterUrlSynchronizer(IBackgroundTaskFilterService filterService, IBackgroundTaskFilterUrlService filterUrlService, ISpotifyUserTaskFilterDbService dbService, INavigationService navService)
	{
		_filterService = filterService;
		_filterUrlService = filterUrlService;
		_dbService = dbService;
		_navService = navService;

		_filterService.OnFilterChanged += OnFilterChanged;
	}

	public void Dispose()
	{
		_filterService.OnFilterChanged -= OnFilterChanged;
		GC.SuppressFinalize(this);
	}

	private void OnFilterChanged()
	{
		if (_suppressUrlUpdate)
		{
			return;
		}

		ChangeFilter();
	}

	public async Task SetFilterFromUrl(string? urlParams, string? searchParam)
	{
		var filter = _filterUrlService.ParseFilterFromUrlParams(urlParams);

		_suppressUrlUpdate = true;
		try
		{
			_filterService.SetFilterAndSearch(filter, searchParam);
		}
		finally
		{
			_suppressUrlUpdate = false;
		}

		// save to db
		// TODO cancel token
		var filterModel = new BackgroundTaskFilter(filter);
		await _dbService.Save(filterModel, true, default);
	}

	private void ChangeFilter()
	{
		var paramaters = _filterUrlService.CreateUrlParams(_filterService.Filter, _filterService.SearchText);
		var url = $"{_baseUrl}{paramaters}";

		_navService.Navigate(url, false, true, "TaskFilterChanged");
	}

	public async Task<string> GetInitUrl()
	{
		// TODO cancel token
		var filterDb = await _dbService.Get(default) ?? new();

		var parameters = _filterUrlService.CreateUrlParams(filterDb.TaskFilter, null);

		var url = $"{_baseUrl}{parameters}";

		return url;
	}
}
