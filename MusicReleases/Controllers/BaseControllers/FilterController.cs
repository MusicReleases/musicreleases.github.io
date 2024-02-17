using Blazored.LocalStorage;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;

public class FilterController(ILocalStorageService localStorage)
{
	private const ServiceType _serviceType = ServiceType.Spotify;

	private readonly ILocalStorageService _localStorage = localStorage;

	// GetLocalStorageKeyReleases

	public async Task SaveFilterToStorage(ReleasesFilters releasesFilter, bool filterEnabled)
	{
		var localStorageKey = GetLocalStorageKeyReleases(_serviceType, releasesFilter);

		await _localStorage.SetItemAsync(localStorageKey, filterEnabled);
	}

	public async Task<bool?> GetSavedFilter(ReleasesFilters releasesFilter)
	{
		var localStorageKey = GetLocalStorageKeyReleases(_serviceType, releasesFilter);
		var filterEnabled = await _localStorage.GetItemAsync<bool>(localStorageKey);
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
}
