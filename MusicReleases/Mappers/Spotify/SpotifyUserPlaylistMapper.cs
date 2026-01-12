using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class SpotifyUserPlaylistMapper
{
	public static SpotifyUserPlaylistEntity ToUserPlaylistEntity(this string playlistId, string userId, int order)
	{
		return new SpotifyUserPlaylistEntity(userId, playlistId, order);
	}
}