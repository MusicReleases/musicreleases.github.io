using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class SpotifyTaskFilterUrlSynchronizer : IDisposable, ISpotifyTaskFilterUrlSynchronizer
{
	private readonly ISpotifyTaskFilterService _filterService;

	private readonly ISpotifyTaskFilterUrlService _filterUrlService;

	private readonly IDbSpotifyUserLinkService _dbService;

	private readonly ISpotifyApiUserService _spotifyApiUserService;

	private readonly NavigationManager _navManager;


	public SpotifyTaskFilterUrlSynchronizer(ISpotifyTaskFilterService filterService, ISpotifyTaskFilterUrlService filterUrlService, IDbSpotifyUserLinkService dbService, ISpotifyApiUserService spotifyApiUserService, NavigationManager navManager)
	{
		_filterService = filterService;
		_filterUrlService = filterUrlService;
		_dbService = dbService;
		_spotifyApiUserService = spotifyApiUserService;
		_navManager = navManager;

		_filterService.OnFilterChanged += OnFilterChanged;
	}

	private const string _baseUrl = "/tasks";

	public async Task SetFilterFromUrl(string? urlParams, string? searchParam)
	{
		var filter = _filterUrlService.ParseFilterFromUrlParams(urlParams);

		_filterService.SetFilterAndSearch(filter, searchParam);

		// url parameters for db - doesnt save search text
		var urlDb = _filterUrlService.CreateUrlParams(_filterService.Filter, null);
		var userId = _spotifyApiUserService.GetUserIdRequired();

		await _dbService.SetTasksLink(userId, urlDb);
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
		_filterService.OnFilterChanged -= OnFilterChanged;
		GC.SuppressFinalize(this);
	}

	public async Task<string> GetInitUrl()
	{
		var userId = _spotifyApiUserService.GetUserIdRequired();
		var parameters = await _dbService.GetTasksLink(userId);
		var url = $"{_baseUrl}{parameters}";

		return url;
	}
}
