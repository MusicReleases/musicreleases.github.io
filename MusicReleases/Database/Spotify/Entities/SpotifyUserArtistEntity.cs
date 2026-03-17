using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserArtist")]
[CompoundIndex(nameof(UserId), nameof(ArtistId), IsPrimary = true)]
public partial record SpotifyUserArtistEntity
(
	[property: Index] string UserId,
	[property: Index] string ArtistId
) : ISpotifyDb, ISpotifyUserIdLinkEntity
{
	public string GetLinkedId() => ArtistId;
}