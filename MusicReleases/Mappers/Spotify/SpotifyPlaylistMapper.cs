using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class SpotifyPlaylistMapper
{
	public static SpotifyPlaylistEntity ToEntity(this SpotifyPlaylist dto)
	{
		return new(dto.Id, dto.Name, dto.UrlApp, dto.UrlWeb, dto.SnapshotId, dto.OwnerId, dto.Collaborative);
	}

	public static SpotifyPlaylist ToModel(this SpotifyPlaylistEntity entity)
	{
		return new(entity.Id, entity.Name, entity.UrlApp, entity.UrlWeb, entity.SnapshotId, entity.OwnerId, entity.Collaborative);
	}
}
