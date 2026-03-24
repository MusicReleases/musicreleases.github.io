using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.User
{
	public interface ISpotifyUserDbService
	{
		Task Delete(string userId);
		Task<SpotifyUser?> Get(string userId, DateTime lastUpdate);
		Task Save(SpotifyUser user);
	}
}