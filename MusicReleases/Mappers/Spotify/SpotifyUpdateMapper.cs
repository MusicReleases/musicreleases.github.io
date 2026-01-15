using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class SpotifyUpdateMapper
{
	public static SpotifyUserUpdateEntity ToSpotifyUpdateEntity(this string userId, SpotifyDbUpdateType type)
	{
		var key = userId.ToSpotifyUpdateKey(type);
		return new(key, DateTime.Now);
	}

	public static string ToSpotifyUpdateKey(this string userId, SpotifyDbUpdateType type)
	{
		return $"{userId}_{type}";
	}
}