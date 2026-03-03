using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.User;

public class SpotifyUserEntity : SpotifyIdNameUrlEntity
{
	public string? ProfilePictureUrl { get; init; }
	public required string RefreshToken { get; init; }

	public SpotifyUserEntity() { }

	[SetsRequiredMembers]
	public SpotifyUserEntity(SpotifyUserInfo spotifyUserInfo, string refreshToken)
	{
		Id = spotifyUserInfo.Id;
		Name = spotifyUserInfo.Name;
		UrlApp = spotifyUserInfo.UrlApp;
		UrlWeb = spotifyUserInfo.UrlWeb;
		ProfilePictureUrl = spotifyUserInfo.ProfilePictureUrl;
		RefreshToken = refreshToken;
	}
}