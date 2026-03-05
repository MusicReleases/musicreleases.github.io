using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserPlaylist")]
[CompoundIndex(nameof(UserId), nameof(PlaylistId), IsPrimary = true)]
public partial record SpotifyUserPlaylistEntity
(
	[property: Index] string UserId,
	[property: Index] string PlaylistId,
	int Order
) : ISpotifyDb;