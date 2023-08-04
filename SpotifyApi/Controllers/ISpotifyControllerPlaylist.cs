using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerPlaylist
{
	Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false);
	Task<ISet<SpotifyPlaylist>> GetUserPlaylists(bool onlyEditable = false);
}