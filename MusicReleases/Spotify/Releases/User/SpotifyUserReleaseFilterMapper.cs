using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Spotify.Releases.User;

public static class SpotifyUserReleaseFilterMapper
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
