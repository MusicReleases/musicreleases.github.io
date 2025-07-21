using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyFilterService
{
	SortedSet<SpotifyArtist>? FilteredArtists { get; }
	SortedSet<SpotifyRelease> FilteredReleases { get; }
	Dictionary<int, SortedSet<int>>? FilteredYearMonth { get; }

	event Action? OnFilterOrDataChanged;

	void SetArtists(ISet<SpotifyArtist> artists);
	void SetReleases(ISet<SpotifyRelease> releases);
}