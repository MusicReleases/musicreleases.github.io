using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "Release")]
[CompoundIndex(nameof(Id), nameof(ReleaseType))]
public partial record SpotifyReleaseEntity
(
	[property: Index(IsPrimary = true)] string Id,
	[property: Index] string Name,
	[property: Index] ReleaseType ReleaseType,
	DateTime ReleaseDate,
	string UrlApp,
	string UrlWeb,
	string UrlImage,
	int TotalTracks
) : ISpotifyDb, ISpotifyIdNameUrlEntity;