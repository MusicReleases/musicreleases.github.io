using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyTaskFilterService
	{
		Task Delete(string userId);
		Task<TaskFilter?> Get(string userId);
		Task Save(TaskFilter filter, string userId);
	}
}