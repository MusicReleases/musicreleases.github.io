using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyUserPlaylistService
	{
		Task Delete(string userId);
		Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>?> Get(string userId);
		Task Save(string userId, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> playlists);
	}
}