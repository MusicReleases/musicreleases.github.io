using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Spotify.Tasks.User;

public static class SpotifyUserTaskFilterMapper
{
	public static SpotifyUserFilterTaskEntity ToEntity(this SpotifyTaskFilter filter, string userId)
	{
		return new(userId, filter.TaskFilter);
	}

	public static SpotifyTaskFilter ToModel(this SpotifyUserFilterTaskEntity entity)
	{
		return new(entity.Filter);
	}
}
