using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserArtist")]
[CompoundIndex(nameof(UserId), nameof(ArtistId), IsPrimary = true)]
public partial record SpotifyUserArtistEntity
(
	[property: Index] string UserId,
	string ArtistId
) : ISpotifyDb;