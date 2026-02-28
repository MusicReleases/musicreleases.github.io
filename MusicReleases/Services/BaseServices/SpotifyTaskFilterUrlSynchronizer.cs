using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyTaskFilterUrlSynchronizer(ISpotifyTaskFilterService filterService, ISpotifyTaskFilterUrlService filterUrlService, IDbSpotifyUserLinkService dbService, ISpotifyApiUserService spotifyApiUserService, NavigationManager navManager) : IDisposable, ISpotifyTaskFilterUrlSynchronizer
{
	private readonly ISpotifyTaskFilterService _filterService = filterService;

	private readonly ISpotifyTaskFilterUrlService _filterUrlService = filterUrlService;

	private readonly IDbSpotifyUserLinkService _dbService = dbService;

	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;

	private readonly NavigationManager _navManager = navManager;


	private const string _baseUrl = "/tasks";

	private bool _isSubscribed = false;

	public async Task SetFilterFromUrl(string? urlParams, string? searchParam)
	{
		var filter = _filterUrlService.ParseFilterFromUrlParams(urlParams);

		_filterService.SetFilterAndSearch(filter, searchParam);

		// url parameters for db - doesnt save search text
		var urlDb = _filterUrlService.CreateUrlParams(_filterService.Filter, null);
		var userId = _spotifyApiUserService.GetUserIdRequired();

		await _dbService.SetTasksLink(userId, urlDb);

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
		var paramaters = _filterUrlService.CreateUrlParams(_filterService.Filter, _filterService.SearchText);
		var url = $"{_baseUrl}{paramaters}";
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

	public async Task<string> GetInitUrl()
	{
		var userId = _spotifyApiUserService.GetUserIdRequired();
		var parameters = await _dbService.GetTasksLink(userId);
		var url = $"{_baseUrl}{parameters}";

		return url;
	}
}
