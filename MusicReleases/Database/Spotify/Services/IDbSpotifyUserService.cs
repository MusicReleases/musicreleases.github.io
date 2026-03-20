using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyUserService
	{
		Task Delete(string userId);
		Task<SpotifyUser?> Get(string userId, DateTime lastUpdate);
		Task Save(SpotifyUser user);
	}
}