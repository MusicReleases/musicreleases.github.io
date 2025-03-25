using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseUserControllerOld
{
	Task Delete(string userId);
	Task DeleteAllDatabases(string userId);
	Task<SpotifyUser?> Get(string userId);
	Task Save(SpotifyUser user);
}