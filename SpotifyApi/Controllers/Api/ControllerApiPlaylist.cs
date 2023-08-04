using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiPlaylist
{
	private readonly SpotifyClient _client;
	private readonly SpotifyUser _user;

	public ControllerApiPlaylist(SpotifyClient client, SpotifyUser user)
	{
		_client = client;
		_user = user;
	}

	public async Task<HashSet<SpotifyPlaylist>> GetUserPlaylistsFromApi()
	{
		var playlistsFromApi = await GetUserPlaylistsApi();
		var playlists = new HashSet<SpotifyPlaylist>();

		if (playlistsFromApi == null)
		{
			return playlists;
		}

		foreach (var playlistApi in playlistsFromApi)
		{
			var currentUserOwned = IsPlaylistOwnedByCurrentUser(playlistApi);
			var playlist = new SpotifyPlaylist(playlistApi, currentUserOwned);
			playlists.Add(playlist);
		}

		return playlists;
	}

	private async Task<IList<SimplePlaylist>?> GetUserPlaylistsApi()
	{
		var request = new PlaylistCurrentUsersRequest
		{
			Limit = ApiRequestLimit.UserPlaylists,
		};
		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Playlists.CurrentUsers(request);
		var playlists = await spotifyClient.PaginateAll(response);

		return playlists;
	}

	private bool IsPlaylistOwnedByCurrentUser(SimplePlaylist playlistApi)
	{
		var playlistOwnerId = playlistApi.Owner.Id;
		if (string.IsNullOrEmpty(playlistOwnerId))
		{
			return false;
		}
		return playlistOwnerId == _user.Id;
	}
}
