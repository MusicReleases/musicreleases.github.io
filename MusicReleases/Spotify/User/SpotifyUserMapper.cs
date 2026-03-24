using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.User;

namespace JakubKastner.MusicReleases.Spotify.User;

public static class SpotifyUserMapper
{
	public static SpotifyUserEntity ToEntity(this SpotifyUser dto)
	{
		return new(dto.Info.Id, dto.Info.Name, dto.Info.UrlApp, dto.Info.UrlWeb, dto.Info.ProfilePictureUrl, dto.Credentials.RefreshToken);
	}

	public static SpotifyUser ToModel(this SpotifyUserEntity entity, DateTime lastUpdate)
	{
		var info = new SpotifyUserInfo(entity.Id, entity.Name, entity.UrlApp, entity.UrlWeb, entity.UrlProfilePicture, lastUpdate);
		var credentials = new SpotifyUserCredentials(entity.RefreshToken);

		return new(info, credentials);
	}
}
