using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseArtistsController
	{
		Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> Get(string userId, bool getReleases);
		Task SaveArtists(string userId, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> artists);
	}
}