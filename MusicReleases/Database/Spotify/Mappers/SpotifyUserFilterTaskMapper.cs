using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyUserFilterTaskMapper
{
	public static SpotifyUserFilterTaskEntity ToEntity(this BackgroundTaskFilter filter, string userId)
	{
		return new(userId, filter.TaskFilter);
	}

	public static BackgroundTaskFilter ToModel(this SpotifyUserFilterTaskEntity entity)
	{
		return new(entity.Filter);
	}
}
