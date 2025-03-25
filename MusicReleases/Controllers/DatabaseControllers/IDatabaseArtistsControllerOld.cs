using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseArtistsControllerOld
	{
		Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> GetFollowed(string userId, bool getReleases);
		Task SaveArtists(string userId, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> artists);
	}
}