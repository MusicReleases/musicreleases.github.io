using DexieNET;
using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "ArtistRelease")]
[CompoundIndex(nameof(ArtistId), nameof(ReleaseId), IsPrimary = true)]
[CompoundIndex(nameof(ArtistId), nameof(Role))]
[CompoundIndex(nameof(ReleaseId), nameof(Role))]
public partial record SpotifyArtistReleaseEntity
(
	[property: Index] string ArtistId,
	[property: Index] string ReleaseId,
	[property: Index] ArtistReleaseRole Role
) : ISpotifyDb;