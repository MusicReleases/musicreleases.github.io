using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "Track")]
public partial record SpotifyTrackEntity
(
	[property: Index(IsPrimary = true)] string Id,
	[property: Index] string Name,
	[property: Index] string ReleaseId,
	int TrackNumber,
	int DiscNumber,
	TimeSpan Duration,
	bool Explicit,
	string UrlApp,
	string UrlWeb
) : ISpotifyDb, ISpotifyIdNameUrlEntity;