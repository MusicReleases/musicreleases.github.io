using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyArtistTrackMapper
{
	public static SpotifyArtistTrackEntity ToArtistTrackEntity(this string trackId, string artistId)
	{
		return new(artistId, trackId);
	}
}