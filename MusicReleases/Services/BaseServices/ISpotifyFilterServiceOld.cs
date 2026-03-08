using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyFilterServiceOld
{
	SpotifyReleaseFilter? Filter { get; }

	SortedSet<SpotifyArtist>? FilteredArtists { get; }
	SortedSet<SpotifyRelease> FilteredReleases { get; }
	Dictionary<int, SortedSet<int>>? FilteredYearMonth { get; }

	event Action? OnFilterOrDataChanged;

	void SetArtists(ISet<SpotifyArtist> artists);
	void SetFilter(SpotifyReleaseFilter filter);
	Task SetFilterAndSaveDb(SpotifyReleaseFilter filter);
	void ClearFilter();
	bool IsFilterActive(FilterType filterType);
	void SetReleases(Dictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> releasesByType);
}