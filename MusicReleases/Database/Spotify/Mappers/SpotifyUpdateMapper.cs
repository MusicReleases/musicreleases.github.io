using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyUpdateMapper
{
	public static SpotifyUserUpdateEntity ToSpotifyUpdateEntity(this string userId, SpotifyDbUpdateType updateType)
	{
		var key = SpotifyUserUpdateEntity.MakeKey(userId, updateType);
		return new(key, userId, updateType, DateTime.Now);
	}
}