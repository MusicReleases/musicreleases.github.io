using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "Playlist")]
public partial record SpotifyPlaylistEntity
	(
		[property: Index(IsPrimary = true)] string Id,
		[property: Index] string Name,
		string UrlApp,
		string UrlWeb,
		string SnapshotId,
		string OwnerId,
		bool Collaborative
	) : ISpotifyDb, ISpotifyIdNameUrlEntity;