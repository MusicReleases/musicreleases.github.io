using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyUserPlaylistMapper
{
	public static SpotifyUserPlaylistEntity ToUserPlaylistEntity(this string playlistId, string userId, int order)
	{
		return new(userId, playlistId, order);
	}
}