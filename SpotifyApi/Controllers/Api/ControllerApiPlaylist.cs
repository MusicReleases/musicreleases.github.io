using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiPlaylist
{
	private readonly Client _client;
	public ControllerApiPlaylist(Client client)
	{
		_client = client;
	}

	public async Task<HashSet<Playlist>> GetUserPlaylistsFromApi()
	{
		HashSet<Playlist> playlists = new();
		var playlistsFromApi = await GetUserPlaylistsApi();

		if (playlistsFromApi == null) return playlists;

		foreach (var playlistApi in playlistsFromApi)
		{
			Playlist playlist = new(simplePlaylist: playlistApi);
			playlists.Add(playlist);
		}

		return playlists;
	}

	private async Task<IList<SimplePlaylist>?> GetUserPlaylistsApi()
	{
		var request = new PlaylistCurrentUsersRequest
		{
			Limit = 50
		};

		var client = _client.GetClient();

		var response = await client.Playlists.CurrentUsers(request);
		var playlists = await client.PaginateAll(response);

		return playlists;
	}
}
