using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.State.Spotify;

public interface ISpotifyArtistState
{
	IReadOnlyList<SpotifyArtist> SortedFollowedArtists { get; }
	DateTime? LastSync { get; }

	event Action? OnChange;

	void SetFollowed(IEnumerable<SpotifyArtist> artists, DateTime lastSync);
}