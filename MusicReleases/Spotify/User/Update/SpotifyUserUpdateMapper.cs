using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Spotify.User.Update;

public static class SpotifyUserUpdateMapper
{
	public static SpotifyUserUpdateEntity ToSpotifyUpdateEntity(this string userId, SpotifyDbUpdateType updateType)
	{
		var key = SpotifyUserUpdateEntity.MakeKey(userId, updateType);
		return new(key, userId, updateType, DateTime.Now);
	}
}