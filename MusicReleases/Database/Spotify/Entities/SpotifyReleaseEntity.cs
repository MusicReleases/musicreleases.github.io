using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "Release")]
public partial record SpotifyReleaseEntity
(
	[property: Index(IsPrimary = true)] string Id,
	[property: Index] string Name,
	ReleaseType ReleaseType,
	DateTime ReleaseDate,
	string UrlApp,
	string UrlWeb,
	string UrlImage,
	int TotalTracks
) : ISpotifyDb, ISpotifyIdNameUrlEntity;