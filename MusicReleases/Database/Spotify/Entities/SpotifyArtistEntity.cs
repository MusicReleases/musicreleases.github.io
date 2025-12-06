using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "Artists")]
public partial record SpotifyArtistEntity
	(
		[property: Index(IsPrimary = true)] string Id,
		[property: Index] string Name,
		string UrlApp,
		string UrlWeb
	) : ISpotifyDb, ISpotifyIdNameUrlEntity;