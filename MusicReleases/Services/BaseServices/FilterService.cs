using Blazored.LocalStorage;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class FilterService(ILocalStorageService localStorageService) : IFilterService
{
	private const ServiceType _serviceType = ServiceType.Spotify;

	private readonly ILocalStorageService _localStorageService = localStorageService;

	// TODO load from db or local storage
	//private readonly SpotifyFilter _filter = new();

	//public SpotifyFilter SpotifyFilter => _filter;

	// GetLocalStorageKeyReleases

	public async Task SaveFilterToStorage(ReleasesFilters releasesFilter, bool filterEnabled)
	{
		var localStorageKey = GetLocalStorageKeyReleases(_serviceType, releasesFilter);

		await _localStorageService.SetItemAsync(localStorageKey, filterEnabled);
	}

	public async Task<bool?> GetSavedFilter(ReleasesFilters releasesFilter)
	{
		var localStorageKey = GetLocalStorageKeyReleases(_serviceType, releasesFilter);
		var filterEnabled = await _localStorageService.GetItemAsync<bool>(localStorageKey);
		return filterEnabled;
	}

	public async Task LoadSavedFilter(ReleasesFilters releasesFilter)
	{
		var filterEnabled = await GetSavedFilter(releasesFilter) ?? false;
		await EnableOrDisableFilter(releasesFilter, filterEnabled, false);
	}

	public async Task EnableOrDisableFilter(ReleasesFilters releasesFilter, bool filterEnabled, bool saveToStorage)
	{
		// TODO enable / disable filter



		if (saveToStorage)
		{
			await SaveFilterToStorage(releasesFilter, filterEnabled);
		}
	}

	/*public void FilterReleaseType(ReleaseType releaseType)
	{
		_filter.ReleaseType = releaseType;
	}
	public void FilterArtist(SpotifyArtist artist)
	{
		_filter.Artist = artist;
	}
	public void FilterYear(int year)
	{
		_filter.Year = year;
		_filter.Month = null;
	}
	public void FilterMonth(int year, int month)
	{
		_filter.Year = null;
		_filter.Month = new DateTime(year, month, 1);
	}*/
}
