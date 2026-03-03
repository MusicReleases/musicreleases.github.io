using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Objects.User;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class SpotifyUserSettingsMapper
{
	public static SpotifyUserSettingsEntity ToEntity(this UserSettings dto, string userId)
	{
		return new(userId, dto.Theme, dto.OpenLinksInApp, dto.PlaylistNewTrackPositionLast, dto.PlaylistAddToProfile);
	}

	public static UserSettings ToModel(this SpotifyUserSettingsEntity entity)
	{
		return new(entity.Theme, entity.OpenLinksInApp, entity.PlaylistNewTrackPositionLast, entity.PlaylistAddToProfile);
	}
}
