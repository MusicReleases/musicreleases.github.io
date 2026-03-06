using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserFilterTask")]
public partial record SpotifyUserFilterTaskEntity
	(
		[property: Index(IsPrimary = true)] string UserId,
		TaskFilter Filter
	) : ISpotifyDb, ISpotifyUserIdEntity;