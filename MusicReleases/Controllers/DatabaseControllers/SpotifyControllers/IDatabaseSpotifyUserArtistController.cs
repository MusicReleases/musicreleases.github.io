using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers
{
	public interface IDatabaseSpotifyUserArtistController
	{
		Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>?> Get(string userId);
		Task Save(string userId, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> artists);
	}
}