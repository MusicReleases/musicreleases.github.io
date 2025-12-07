using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserArtist")]
[CompoundIndex(nameof(UserId), nameof(ArtistId), IsPrimary = true)]
public partial record SpotifyUserArtistEntity
(
	[property: Index] string UserId,
	string ArtistId
) : ISpotifyDb;


/*
public partial record SpotifyUserArtistEntity
(
	[property: Index(IsPrimary = true)]
	(string UserId, string ArtistId) Key,
	[property: Index] string UserId
) : ISpotifyDb;
*/