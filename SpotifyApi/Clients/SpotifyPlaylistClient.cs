using JakubKastner.SpotifyApi.Store;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Clients;

internal class SpotifyPlaylistClient(ISpotifyClientStore client) : ISpotifyPlaylistClient
{
	private readonly ISpotifyClientStore _client = client;

	public async Task<List<SpotifyPlaylist>> GetUserPlaylists(CancellationToken ct = default)
	{
		var request = new PlaylistCurrentUsersRequest
		{
			Limit = ApiRequestLimit.UserPlaylists,
		};

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Playlists.CurrentUsers(request, ct);

		var playlistsAsync = spotifyClient.Paginate(response, cancel: ct);

		var playlists = new List<SpotifyPlaylist>();
		await foreach (var playlistApi in playlistsAsync.WithCancellation(ct))
		{
			var playlist = playlistApi.ToObject();
			playlists.Add(playlist);
		}
		return playlists;
	}

	public async Task<SpotifyPlaylist> CreatePlaylist(string userId, string name, bool addToProfile, CancellationToken ct = default)
	{
		var request = new PlaylistCreateRequest(name)
		{
			Collaborative = false,
			Public = addToProfile, // Public in api = add to user profile - playlist will be public every time
			Description = string.Empty,
		};

		var spotifyClient = _client.GetClient();
		var playlistApi = await spotifyClient.Playlists.Create(request, ct);
		var playlist = playlistApi.ToObject();

		return playlist;
	}

	public async Task<string> AddTracksToPlaylist(string playlistId, IEnumerable<string> trackUris, bool positionTop, CancellationToken ct = default)
	{
		var request = new PlaylistAddItemsRequest([.. trackUris])
		{
			Position = positionTop ? 0 : null
		};

		var spotifyClient = _client.GetClient();
		var snapshot = await spotifyClient.Playlists.AddPlaylistItems(playlistId, request, ct);
		return snapshot.SnapshotId;
	}

	public async Task<string> RemoveTracksFromPlaylist(string playlistId, IEnumerable<string> trackUris, CancellationToken ct = default)
	{
		var request = new PlaylistRemoveItemsRequestV2
		{
			Items = [.. trackUris.Select(uri => new PlaylistRemoveItemsRequestV2.Item { Uri = uri })],
		};

		var spotifyClient = _client.GetClient();
		var snapshot = await spotifyClient.Playlists.RemovePlaylistItems(playlistId, request, ct);
		return snapshot.SnapshotId;
	}
}