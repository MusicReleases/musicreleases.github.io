using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Spotify.Tasks.User;

internal static class SpotifyUserTaskFilterMapper
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
