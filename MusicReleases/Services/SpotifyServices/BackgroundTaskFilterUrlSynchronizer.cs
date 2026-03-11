using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class BackgroundTaskFilterUrlSynchronizer : IDisposable, IBackgroundTaskFilterUrlSynchronizer
{
	private readonly IBackgroundTaskFilterService _filterService;

	private readonly IBackgroundTaskFilterUrlService _filterUrlService;

	private readonly IDbSpotifyUserFilterTaskService _dbService;

	private readonly ISpotifyApiUserService _spotifyApiUserService;

	private readonly NavigationManager _navManager;


	public BackgroundTaskFilterUrlSynchronizer(IBackgroundTaskFilterService filterService, IBackgroundTaskFilterUrlService filterUrlService, IDbSpotifyUserFilterTaskService dbService, ISpotifyApiUserService spotifyApiUserService, NavigationManager navManager)
	{
		_filterService = filterService;
		_filterUrlService = filterUrlService;
		_dbService = dbService;
		_spotifyApiUserService = spotifyApiUserService;
		_navManager = navManager;

		_filterService.OnFilterChanged += OnFilterChanged;
	}

	public void Dispose()
	{
		_filterService.OnFilterChanged -= OnFilterChanged;
		GC.SuppressFinalize(this);
	}


	private const string _baseUrl = "/tasks";


	public async Task SetFilterFromUrl(string? urlParams, string? searchParam)
	{
		var filter = _filterUrlService.ParseFilterFromUrlParams(urlParams);

		_filterService.SetFilterAndSearch(filter, searchParam);

		// save to db
		var userId = _spotifyApiUserService.GetUserIdRequired();
		await _dbService.Save(filter, userId);
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

	public async Task<string> GetInitUrl()
	{
		var userId = _spotifyApiUserService.GetUserIdRequired();

		var filterDb = await _dbService.Get(userId) ?? TaskFilter.All;
		var parameters = _filterUrlService.CreateUrlParams(filterDb, null);

		var url = $"{_baseUrl}{parameters}";

		return url;
	}
}
