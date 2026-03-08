using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Objects;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class SpotifyUserFilterReleaseMapper
{
	public static SpotifyUserFilterReleaseEntity ToEntity(this SpotifyReleaseFilter dto, string userId)
	{
		return new(userId, dto.ReleaseType, dto.ReleaseAdvancedFilter, dto.Artist, dto.Year, dto.Month);
	}

	public static SpotifyReleaseFilter ToModel(this SpotifyUserFilterReleaseEntity entity)
	{
		return new(entity.ReleaseType, entity.ReleaseAdvancedFilter, entity.ArtistId, entity.Year, entity.Month);
	}
}
