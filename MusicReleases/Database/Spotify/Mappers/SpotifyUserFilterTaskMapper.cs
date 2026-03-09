using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyUserFilterTaskMapper
{
	public static SpotifyUserFilterTaskEntity ToEntity(this TaskFilter filter, string userId)
	{
		return new(userId, filter);
	}

	public static TaskFilter ToModel(this SpotifyUserFilterTaskEntity entity)
	{
		return entity.Filter;
	}
}
