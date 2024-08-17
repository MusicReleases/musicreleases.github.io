using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public interface IControllerApiPlaylist
{
	Task<ISet<SpotifyPlaylist>> GetUserPlaylistsFromApi(ISet<SpotifyPlaylist>? existingPlaylists = null);
	Task<ISet<SpotifyPlaylist>> GetPlaylistsTracksFromApi(ISet<SpotifyPlaylist> playlists, bool forceUpdate);
	Task<IList<SpotifyTrack>> GetPlaylistTracksFromApi(string playlistId);
}