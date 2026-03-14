using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyUserMapper
{
	public static SpotifyUserEntity ToEntity(this SpotifyUser dto)
	{
		return new(dto.Info.Id, dto.Info.Name, dto.Info.UrlApp, dto.Info.UrlWeb, dto.Info.ProfilePictureUrl, dto.Credentials.RefreshToken);
	}

	public static SpotifyUser ToModel(this SpotifyUserEntity entity)
	{
		var info = new SpotifyUserInfo(entity.Id, entity.Name, entity.UrlApp, entity.UrlWeb, entity.UrlProfilePicture);
		var credentials = new SpotifyUserCredentials(entity.RefreshToken);

		return new(info, credentials);
	}
}
