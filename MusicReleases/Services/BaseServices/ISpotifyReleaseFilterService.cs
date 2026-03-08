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

		//event Action? OnFilterOrDataChanged;
		event Action? OnFilterChanged;
		event Action? OnDataFiltered;

		void ApplyFilterAndSearch();
		void ClearFilter(FilterType type);
		ReleaseAdvancedFilter EnsureAdvancedFilter(ReleaseAdvancedFilter newFilter);
		void EnsureFilter(SpotifyFilter filter);
		string? EnsureSearchText(string? searchText);
		void FilterArtist(string? artistId);
		void FilterMonth(int? year, int? month);
		void FilterReleaseType(MainReleasesType releaseType);
		void FilterYear(int? year);
		bool IsAdvancedFilterActive(ReleaseAdvancedFilter advancedFilter);
		bool IsFilterActive(FilterType filterType);
		void SeAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
		void SetFromUrl(SpotifyFilter newFilter);
		void ToggleAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
		void UnsetAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
	}
}