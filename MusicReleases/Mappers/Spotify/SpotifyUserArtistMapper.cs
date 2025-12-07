using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class SpotifyUserArtistMapper
{
	public static SpotifyUserArtistEntity ToSpotifyUserArtistEntity(this string artistId, string userId)
	{
		return new(userId, artistId);
	}

	public static SpotifyUserArtistEntity ToSpotifyUserArtistEntity(this SpotifyArtist artist, string userId)
	{
		return new(userId, artist.Id);
	}
}