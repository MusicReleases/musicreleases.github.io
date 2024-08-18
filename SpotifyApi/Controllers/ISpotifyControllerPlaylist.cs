using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerPlaylist
{
	Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>> GetPlaylistsTracks(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? playlistsStorage = null, bool forceUpdate = false);
	Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false);
	Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>> GetUserPlaylists(bool onlyEditable = false, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? existingPlaylist = null, bool forceUpdate = false);
}