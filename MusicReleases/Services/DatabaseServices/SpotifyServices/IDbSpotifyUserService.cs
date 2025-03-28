using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUserService
{
	Task Delete(string userId);
	Task DeleteAllUserDatabases(string userId);
	Task<SpotifyUser?> Get(string userId);
	Task Save(SpotifyUser user);
}