using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Spotify.Artists.Tracks;

internal static class SpotifyArtistTrackMapper
{
	public static SpotifyArtistTrackEntity ToArtistTrackEntity(this string trackId, string artistId)
	{
		return new(artistId, trackId);
	}
}