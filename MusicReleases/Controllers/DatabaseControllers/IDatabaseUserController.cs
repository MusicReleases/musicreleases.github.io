using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseUserController
	{
		Task Delete(string userId);
		Task DeleteAllDatabases(string userId);
		Task<SpotifyUser?> Get(string userId);
		Task Save(SpotifyUser user);
	}
}