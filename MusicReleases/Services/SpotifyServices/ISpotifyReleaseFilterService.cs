using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.SpotifyServices
{
	public interface ISpotifyReleaseFilterService
	{
		SpotifyReleaseFilter Filter { get; }
		SortedSet<SpotifyArtist>? FilteredArtists { get; }
		Dictionary<int, SortedSet<int>>? FilteredDate { get; }
		SortedSet<SpotifyRelease>? FilteredReleases { get; }

		event Action? OnFilterChanged;
		event Action? OnDataFiltered;
		event Action? NotifySynchronizer;

		void ApplyFilterAndSearch();
		void ClearFilter(FilterType type);
		ReleaseAdvancedFilter EnsureAdvancedFilter(ReleaseAdvancedFilter newFilter);
		void EnsureFilter(SpotifyReleaseFilter filter);
		string? EnsureSearchText(string? searchText);
		void FilterArtist(string? artistId);
		void FilterMonth(int? year, int? month);
		void FilterReleaseType(ReleaseGroup releaseType);
		void FilterYear(int? year);
		bool IsAdvancedFilterActive(ReleaseAdvancedFilter advancedFilter);
		bool IsArtistFiltered(string artistId);
		bool IsFilterActive(FilterType filterType);
		void SeAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
		void SetFromUrl(SpotifyReleaseFilter newFilter);
		void SetSearch(string? searchText);
		void ToggleAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
		void UnsetAdvancedFilter(ReleaseAdvancedFilter advancedFilter);
	}
}