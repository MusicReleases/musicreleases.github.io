using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.User;

public class SpotifyUserEntity : SpotifyIdNameEntity
{
	public string? Country { get; set; }
	public string? ProfilePictureUrl { get; set; }
	public string? RefreshToken { get; set; }

	public SpotifyUserEntity() { }

	public SpotifyUserEntity(SpotifyUserInfo spotifyUserInfo, string refreshToken)
	{
		Id = spotifyUserInfo.Id;
		Name = spotifyUserInfo.Name;
		Country = spotifyUserInfo.Country;
		ProfilePictureUrl = spotifyUserInfo.ProfilePictureUrl;
		RefreshToken = refreshToken;
	}
}