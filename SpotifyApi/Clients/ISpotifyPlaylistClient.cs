namespace JakubKastner.SpotifyApi.Clients;

public interface ISpotifyPlaylistClient
{
	Task<string> AddTracksToPlaylist(string playlistId, IEnumerable<string> trackUris, bool positionTop, CancellationToken ct = default);
	Task<SpotifyPlaylist> CreatePlaylist(string userId, string name, bool addToProfile, CancellationToken ct = default);
	Task<List<SpotifyPlaylist>> GetUserPlaylists(CancellationToken ct = default);
	Task<string> RemoveTracksFromPlaylist(string playlistId, IEnumerable<string> trackUris, CancellationToken ct = default);
}