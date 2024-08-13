using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerPlaylist
{
	Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false);
	Task<SpotifyUserList<SpotifyPlaylist>> GetUserPlaylists(bool onlyEditable = false, SpotifyUserList<SpotifyPlaylist>? existingPlaylist = null, bool forceUpdate = false);
}