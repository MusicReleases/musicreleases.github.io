using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities;

[Schema(StoreName = "UserUpdate")]
[CompoundIndex(nameof(UserId), nameof(UpdateType))]
public partial record SpotifyUserUpdateEntity
	(
		[property: Index(IsPrimary = true)] string Key,

		[property: Index] string UserId,
		[property: Index] SpotifyDbUpdateType UpdateType,
		DateTime LastUpdate
	) : ISpotifyDb, ISpotifyUserIdEntity
{
	public static string MakeKey(string userId, SpotifyDbUpdateType updateType)
	{
		return $"{userId}_{updateType}";
	}
}