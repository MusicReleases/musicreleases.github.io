using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "ArtistRelease")]
[CompoundIndex(nameof(ArtistId), nameof(ReleaseId), IsPrimary = true)]

[CompoundIndex(nameof(ArtistId), nameof(Role))]
[CompoundIndex(nameof(ArtistId), nameof(ReleaseType))]
[CompoundIndex(nameof(ArtistId), nameof(Role), nameof(ReleaseType))]

[CompoundIndex(nameof(ReleaseId), nameof(Role))]
[CompoundIndex(nameof(ReleaseId), nameof(ReleaseType))]
[CompoundIndex(nameof(ReleaseId), nameof(Role), nameof(ReleaseType))]
public partial record SpotifyArtistReleaseEntity
(
	[property: Index] string ArtistId,
	[property: Index] string ReleaseId,
	[property: Index] ArtistReleaseRole Role,
	[property: Index] ReleaseType ReleaseType
) : ISpotifyDb, ISpotifyArtistLinkEntity;