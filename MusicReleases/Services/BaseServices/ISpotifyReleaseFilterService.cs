using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ISpotifyReleaseFilterService
	{
		SpotifyFilter Filter { get; }
		SortedSet<SpotifyArtist>? FilteredArtists { get; }
		Dictionary<int, SortedSet<int>>? FilteredDate { get; }
		SortedSet<SpotifyRelease>? FilteredReleases { get; }

		event Action? OnFilterOrDataChanged;

		void ApplyFilterAndSearch();
		void ClearAdvancedFilter();
		void ClearAllFilters();
		void ClearFilter(FilterType type);
		void Dispose();
		void FilterArtist(string? artistId);
		void FilterMonth(int? year, int? month);
		void FilterReleaseType(MainReleasesType releaseType);
		void FilterYear(int? year);
		bool IsAdvancedFilterActive(ReleaseAdvancedFilter advancedFilter);
		bool IsFilterActive(FilterType filterType);
		void SeAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
		void SetFilterAndSearch(SpotifyFilter newFilter, string? newSearchText, bool onChange = false);
		void SetSearch(string? newSearchText);
		void ToggleAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
		void UnsetAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
	}
}