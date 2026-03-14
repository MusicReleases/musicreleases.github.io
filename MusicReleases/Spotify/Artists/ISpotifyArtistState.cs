using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal interface ISpotifyArtistState
{
	IReadOnlySet<SpotifyArtist>? FollowedArtists { get; }
	DateTime? LastSync { get; }

	event Action? OnChange;

	bool IsFollowed(string artistId);
	void SetFollowed(IReadOnlyCollection<SpotifyArtist> artists, DateTime lastSync);
}