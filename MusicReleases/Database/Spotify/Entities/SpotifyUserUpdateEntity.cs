using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "Update")]
public partial record SpotifyUserUpdateEntity
(
	[property: Index(IsPrimary = true)] string Key,
	DateTime LastUpdate
) : ISpotifyDb;