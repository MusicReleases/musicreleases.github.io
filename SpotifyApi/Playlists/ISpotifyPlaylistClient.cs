namespace JakubKastner.SpotifyApi.Playlists;

public interface ISpotifyPlaylistClient
{
	Task<string> AddTracksToPlaylist(string playlistId, IEnumerable<string> trackUris, bool positionTop, CancellationToken ct = default);
	Task<SpotifyPlaylist> CreatePlaylist(string userId, string name, bool addToProfile, int order, CancellationToken ct = default);
	Task<List<SpotifyPlaylist>> GetUserPlaylists(CancellationToken ct = default);
	IAsyncEnumerable<IReadOnlyCollection<SpotifyPlaylist>> GetUserPlaylistsBatches(int batchSize = 25, CancellationToken ct = default);
	Task<string> RemoveTracksFromPlaylist(string playlistId, IEnumerable<string> trackUris, CancellationToken ct = default);
}