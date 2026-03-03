using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Services.Api;

public class ApiPlaylistClient(ISpotifyApiClient client) : IApiPlaylistClient
{
	private readonly ISpotifyApiClient _client = client;

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
			var playlist = new SpotifyPlaylist(playlistApi);
			playlists.Add(playlist);
		}
		return playlists;
	}

	public async Task<SpotifyPlaylist> CreatePlaylist(string userId, string name, bool addToProfile)
	{
		var request = new PlaylistCreateRequest(name)
		{
			Collaborative = false,
			Public = addToProfile, // Public in api = add to user profile - playlist will be public every time
			Description = string.Empty,
		};

		var spotifyClient = _client.GetClient();
		var playlistApi = await spotifyClient.Playlists.Create(userId, request);
		var playlist = new SpotifyPlaylist(playlistApi);

		return playlist;
	}

	public async Task<string> AddTracksToPlaylist(string playlistId, IEnumerable<string> trackUris, bool positionTop)
	{
		var request = new PlaylistAddItemsRequest([.. trackUris])
		{
			Position = positionTop ? 0 : null
		};

		var spotifyClient = _client.GetClient();
		var snapshot = await spotifyClient.Playlists.AddItems(playlistId, request);
		return snapshot.SnapshotId;
	}

	public async Task<string> RemoveTracksFromPlaylist(string playlistId, IEnumerable<string> trackUris)
	{
		var request = new PlaylistRemoveItemsRequest
		{
			Tracks = [.. trackUris.Select(uri => new PlaylistRemoveItemsRequest.Item { Uri = uri })],
		};

		var spotifyClient = _client.GetClient();
		var snapshot = await spotifyClient.Playlists.RemoveItems(playlistId, request);
		return snapshot.SnapshotId;
	}
}