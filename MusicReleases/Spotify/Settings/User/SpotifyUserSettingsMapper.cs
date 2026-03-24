using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Spotify.Settings.User;

public static class SpotifyUserSettingsMapper
{
	public static SpotifyUserSettingsEntity ToEntity(this SpotifySettings dto, string userId)
	{
		return new(userId, dto.Theme, dto.OpenLinksInApp, dto.PlaylistNewTrackPositionLast, dto.PlaylistAddToProfile);
	}

	public static SpotifySettings ToModel(this SpotifyUserSettingsEntity entity)
	{
		return new(entity.Theme, entity.OpenLinksInApp, entity.PlaylistNewTrackPositionLast, entity.PlaylistAddToProfile);
	}
}
