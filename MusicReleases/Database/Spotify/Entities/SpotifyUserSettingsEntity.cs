using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserSettings")]
public partial record SpotifyUserSettingsEntity
	(
		[property: Index(IsPrimary = true)] string UserId,
		Theme Theme,
		bool OpenLinksInApp,
		bool PlaylistNewTrackPositionLast,
		bool PlaylistAddToProfile
	) : ISpotifyDb, ISpotifyUserIdEntity;