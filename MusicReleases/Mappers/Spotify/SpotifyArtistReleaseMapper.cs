using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class SpotifyArtistReleaseMapper
{
	public static SpotifyArtistReleaseEntity ToArtistReleaseEntity(this string releaseId, string artistId, ArtistReleaseRole artistRole)
	{
		return new(artistId, releaseId, artistRole);
	}
}