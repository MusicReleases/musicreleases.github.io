using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserFilterRelease")]
public partial record SpotifyUserFilterReleaseEntity
	(
		[property: Index(IsPrimary = true)] string UserId,
		ReleaseGroup ReleaseType,
		ReleaseAdvancedFilter ReleaseAdvancedFilter,
		string? ArtistId,
		int? Year,
		DateTime? Month
	) : ISpotifyDb, ISpotifyUserIdEntity;