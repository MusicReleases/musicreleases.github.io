using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyTrackMapper
{
	public static SpotifyTrackEntity ToEntity(this SpotifyTrack dto)
	{
		return new(dto.Id, dto.Name, dto.ReleaseId, dto.TrackNumber, dto.DiscNumber, dto.Duration, dto.Explicit, dto.UrlApp, dto.UrlWeb);
	}

	public static SpotifyTrack ToModel(this SpotifyTrackEntity entity)
	{
		return new(entity.Id, entity.Name, entity.UrlApp, entity.UrlWeb, entity.ReleaseId, entity.TrackNumber, entity.DiscNumber, entity.Duration, entity.Explicit);
	}
}
