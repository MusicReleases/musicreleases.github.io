using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "ArtistTrack")]
[CompoundIndex(nameof(ArtistId), nameof(TrackId), IsPrimary = true)]
public partial record SpotifyArtistTrackEntity
(
	[property: Index] string ArtistId,
	[property: Index] string TrackId
) : ISpotifyDb;