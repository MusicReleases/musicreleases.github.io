using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyUserFilterTaskService
	{
		Task Delete(string userId);
		Task<TaskFilter?> Get(string userId);
		Task Save(TaskFilter filter, string userId);
	}
}