using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyUserFilterReleaseMapper
{
	public static SpotifyUserFilterReleaseEntity ToEntity(this SpotifyReleaseFilter dto, string userId)
	{
		return new(userId, dto.ReleaseGroup, dto.ReleaseAdvancedFilter, dto.Artist, dto.Year, dto.Month);
	}

	public static SpotifyReleaseFilter ToModel(this SpotifyUserFilterReleaseEntity entity)
	{
		return new(entity.ReleaseType, entity.ReleaseAdvancedFilter, entity.ArtistId, entity.Year, entity.Month);
	}
}
