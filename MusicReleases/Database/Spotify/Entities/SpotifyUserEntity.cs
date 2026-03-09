using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "User")]
public partial record SpotifyUserEntity
	(
		[property: Index(IsPrimary = true)] string Id,
		[property: Index] string Name,
		string UrlApp,
		string UrlWeb,
		string? UrlProfilePicture,
		string RefreshToken
	) : ISpotifyDb, ISpotifyIdNameUrlEntity;