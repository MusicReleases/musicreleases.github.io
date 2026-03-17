using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal static class SpotifyUserPlaylistMapper
{
	public static SpotifyUserPlaylistEntity ToEntity(this SpotifyUserPlaylistPayload payload, string userId)
	{
		return new(userId, payload.Id, payload.Order);
	}

	public static SpotifyUserPlaylistPayload ToPayload(this SpotifyUserPlaylistEntity entity)
	{
		return new(entity.PlaylistId, entity.Order);
	}
	public static SpotifyUserPlaylistPayload ToPayload(this SpotifyPlaylist model)
	{
		return new(model.Id, model.Order);
	}
}