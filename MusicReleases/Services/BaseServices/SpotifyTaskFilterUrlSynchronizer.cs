using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyTaskFilterUrlSynchronizer(ISpotifyTaskFilterService filterService, ISpotifyTaskFilterUrlService filterUrlService, NavigationManager navManager) : IDisposable, ISpotifyTaskFilterUrlSynchronizer
{
	private readonly ISpotifyTaskFilterService _filterService = filterService;

	private readonly ISpotifyTaskFilterUrlService _filterUrlService = filterUrlService;

	private readonly NavigationManager _navManager = navManager;


	private bool _isSubscribed = false;


	public void SetFilterFromUrl(string? urlParams, string? searchParam)
	{
		var filter = _filterUrlService.ParseFilterFromUrlParams(urlParams);

		_filterService.SetFilterAndSearch(filter, searchParam);

		if (!_isSubscribed)
		{
			_filterService.OnFilterChanged += OnFilterChanged;
			_isSubscribed = true;
		}
	}

	private void OnFilterChanged()
	{
		ChangeFilter();
	}

	private void ChangeFilter()
	{
		var url = _filterUrlService.CreateUrlParams(_filterService.Filter, _filterService.SearchText);
		url = $"/tasks{url}";
		_navManager.NavigateTo(url, false);
	}

	public void Dispose()
	{
		if (_isSubscribed)
		{
			_filterService.OnFilterChanged -= OnFilterChanged;
			GC.SuppressFinalize(this);
			_isSubscribed = false;
		}
	}

}
