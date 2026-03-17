using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserFilterRelease")]
public partial record SpotifyUserFilterReleaseEntity
	(
		[property: Index(IsPrimary = true)] string UserId,
		ReleaseEnums ReleaseType,
		ReleaseAdvancedFilter ReleaseAdvancedFilter,
		string? ArtistId,
		int? Year,
		DateTime? Month
	) : ISpotifyDb, ISpotifyUserIdEntity;