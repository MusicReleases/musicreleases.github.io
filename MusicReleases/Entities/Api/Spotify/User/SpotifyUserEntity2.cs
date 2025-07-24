using DexieNET;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.User;

[DBName("SpotifyUserDb")]
public interface ISpotifyUserDB : IDBStore { }

public partial class SpotifyUserEntity2 : ISpotifyUserDB, ISpotifyIdNameEntity
{
	[Index]
	public required string SpotifyId { get; init; }
	[Index]
	public required string Name { get; init; }
	public required string Country { get; init; }
	public string? ProfilePictureUrl { get; init; }
	public required string RefreshToken { get; init; }

	public SpotifyUserEntity2() { }

	[SetsRequiredMembers]
	public SpotifyUserEntity2(SpotifyUserInfo spotifyUserInfo, string refreshToken)
	{
		SpotifyId = spotifyUserInfo.Id;
		Name = spotifyUserInfo.Name;
		Country = spotifyUserInfo.Country;
		ProfilePictureUrl = spotifyUserInfo.ProfilePictureUrl;
		RefreshToken = refreshToken;
	}
}