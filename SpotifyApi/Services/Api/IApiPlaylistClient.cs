using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

public interface IApiPlaylistClient
{
	Task<string> AddTracksToPlaylist(string playlistId, IEnumerable<string> trackUris, bool positionTop);
	Task<SpotifyPlaylist> CreatePlaylist(string userId, string name);
	Task<List<SpotifyPlaylist>> GetUserPlaylists(CancellationToken ct = default);
	Task<string> RemoveTracksFromPlaylist(string playlistId, IEnumerable<string> trackUris);
}