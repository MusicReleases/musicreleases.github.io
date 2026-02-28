using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "Link")]
public partial record SpotifyUserLinkEntity
	(
		[property: Index(IsPrimary = true)] string UserId,
		string? Tasks,
		string? Releases
	) : ISpotifyDb;