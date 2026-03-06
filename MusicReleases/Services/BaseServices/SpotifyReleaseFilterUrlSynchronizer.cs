using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyReleaseFilterUrlSynchronizer(ISpotifyReleaseFilterService filterService, ISpotifyReleaseFilterUrlService filterUrlService, IDbSpotifyReleaseFilterService dbService, ISpotifyApiUserService spotifyApiUserService, NavigationManager navManager) : IDisposable, ISpotifyReleaseFilterUrlSynchronizer
{
	private readonly ISpotifyReleaseFilterService _filterService = filterService;

	private readonly ISpotifyReleaseFilterUrlService _filterUrlService = filterUrlService;

	private readonly IDbSpotifyReleaseFilterService _dbService = dbService;

	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;

	private readonly NavigationManager _navManager = navManager;


	private const string _baseUrl = "/releases/";

	private bool _isSubscribed = false;

	public async Task SetFilterFromUrl(string? releaseType, string? year, string? month, string? artist, string? advancedFilterParams, string? searchParam)
	{
		SubscribeFilterChanges();

		var filter = _filterUrlService.ParseFilterFromUrlParams(releaseType, year, month, artist, advancedFilterParams);

		_filterService.SetFilterAndSearch(filter, searchParam);

		var userId = _spotifyApiUserService.GetUserIdRequired();

		await _dbService.Save(filter, userId);

	}

	private void OnFilterChanged()
	{
		ChangeFilter();
	}

	private void ChangeFilter()
	{
		var paramaters = _filterUrlService.CreateUrl(_filterService.Filter);
		var url = $"{_baseUrl}{paramaters}";
		_navManager.NavigateTo(url, false);
	}

	public void Dispose()
	{
		if (!_isSubscribed)
		{
			return;
		}

		_filterService.Dispose();
		_filterService.OnFilterOrDataChanged -= OnFilterChanged;
		GC.SuppressFinalize(this);
		_isSubscribed = false;
	}

	private void SubscribeFilterChanges()
	{
		if (_isSubscribed)
		{
			return;
		}

		_filterService.OnFilterOrDataChanged += OnFilterChanged;
		_isSubscribed = true;

	}

	public async Task SetInitFilter()
	{
		SubscribeFilterChanges();

		var userId = _spotifyApiUserService.GetUserIdRequired();
		var filter = await _dbService.Get(userId) ?? new();

		_filterService.SetFilterAndSearch(filter, null, true);

		/*var parameters = _filterUrlService.CreateUrl(filter);

		var url = $"{_baseUrl}{parameters}";
		return url;*/
	}
}
