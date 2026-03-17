using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal static class SpotifyPlaylistMapper
{
	public static SpotifyPlaylistEntity ToEntity(this SpotifyPlaylist dto)
	{
		return new(dto.Id, dto.Name, dto.UrlApp, dto.UrlWeb, dto.SnapshotId, dto.OwnerId, dto.Collaborative);
	}

	public static SpotifyPlaylist ToModel(this SpotifyPlaylistEntity entity, SpotifyUserPlaylistPayload payload)
	{
		return new(entity.Id, entity.Name, entity.UrlApp, entity.UrlWeb, entity.SnapshotId, entity.OwnerId, entity.Collaborative, payload.Order);
	}
}
