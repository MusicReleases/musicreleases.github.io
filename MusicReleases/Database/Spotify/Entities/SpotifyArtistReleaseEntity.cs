using DexieNET;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "ArtistRelease")]
[CompoundIndex(nameof(ArtistId), nameof(ReleaseId), IsPrimary = true)]
public partial record SpotifyArtistReleaseEntity
(
	[property: Index] string ArtistId,
	[property: Index] string ReleaseId,
	[property: Index] ArtistReleaseRole Role
) : ISpotifyDb;