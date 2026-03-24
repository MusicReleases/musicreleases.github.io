using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Spotify.User.Tasks;
using JakubKastner.SpotifyApi.Clients;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.BackgroundTasks.Services;

internal sealed class BackgroundTaskFilterUrlSynchronizer : IBackgroundTaskFilterUrlSynchronizer
{
	private readonly IBackgroundTaskFilterService _filterService;
	private readonly IBackgroundTaskFilterUrlService _filterUrlService;
	private readonly ISpotifyUserFilterTaskDbService _dbService;
	private readonly ISpotifyUserClient _spotifyUserClient;
	private readonly NavigationManager _navManager;

	private const string _baseUrl = "/tasks";

	public BackgroundTaskFilterUrlSynchronizer(IBackgroundTaskFilterService filterService, IBackgroundTaskFilterUrlService filterUrlService, ISpotifyUserFilterTaskDbService dbService, ISpotifyUserClient spotifyUserClient, NavigationManager navManager)
	{
		_filterService = filterService;
		_filterUrlService = filterUrlService;
		_dbService = dbService;
		_spotifyUserClient = spotifyUserClient;
		_navManager = navManager;

		_filterService.OnFilterChanged += OnFilterChanged;
	}

	public void Dispose()
	{
		_filterService.OnFilterChanged -= OnFilterChanged;
		GC.SuppressFinalize(this);
	}

	private void OnFilterChanged()
	{
		ChangeFilter();
	}

	public async Task SetFilterFromUrl(string? urlParams, string? searchParam)
	{
		var filter = _filterUrlService.ParseFilterFromUrlParams(urlParams);

		_filterService.SetFilterAndSearch(filter, searchParam);

		// save to db
		// TODO cancel token
		var filterModel = new BackgroundTaskFilter(filter);

		await _dbService.Save(filterModel, true, default);
	}

	private void ChangeFilter()
	{
		var paramaters = _filterUrlService.CreateUrlParams(_filterService.Filter, _filterService.SearchText);
		var url = $"{_baseUrl}{paramaters}";
		_navManager.NavigateTo(url, false);
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
