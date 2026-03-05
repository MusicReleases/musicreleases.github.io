using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyFilterService
{
	SpotifyFilter? Filter { get; }

	SortedSet<SpotifyArtist>? FilteredArtists { get; }
	SortedSet<SpotifyRelease> FilteredReleases { get; }
	Dictionary<int, SortedSet<int>>? FilteredYearMonth { get; }

	event Action? OnFilterOrDataChanged;

	void SetArtists(ISet<SpotifyArtist> artists);
	void SetReleases(ISet<SpotifyRelease> releases);
	void SetFilter(SpotifyFilter filter);
	Task SetFilterAndSaveDb(SpotifyFilter filter);
	void ClearFilter();
	bool IsFilterActive(FilterType filterType);
	void SetReleases(Dictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> releasesByType);
}