using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Spotify.Artists.Releases;

internal static class SpotifyArtistReleaseMapper
{
	public static SpotifyArtistReleaseEntity ToArtistReleaseEntity(this string releaseId, string artistId, ArtistReleaseRole artistRole)
	{
		return new(artistId, releaseId, artistRole);
	}
}