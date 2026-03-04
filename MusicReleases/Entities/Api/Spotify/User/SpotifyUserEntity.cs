using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.User;

public class SpotifyUserEntity : SpotifyIdNameEntity
{
	public required string Country { get; init; }
	public string? ProfilePictureUrl { get; init; }
	public required string RefreshToken { get; init; }

	public SpotifyUserEntity() { }

	[SetsRequiredMembers]
	public SpotifyUserEntity(SpotifyUserInfo spotifyUserInfo, string refreshToken)
	{
		Id = spotifyUserInfo.Id;
		Name = spotifyUserInfo.Name;
		Country = spotifyUserInfo.Country;
		ProfilePictureUrl = spotifyUserInfo.ProfilePictureUrl;
		RefreshToken = refreshToken;
	}
}