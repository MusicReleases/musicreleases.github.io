using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseSpotifyUserController
{
	Task Delete(string userId);
	Task DeleteAllUserDatabases(string userId);
	Task<SpotifyUser?> Get(string userId);
	Task Save(SpotifyUser user);
}