using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class ControllerPlaylist
{
	private readonly ControllerApiPlaylist _controllerApiPlaylist;
	private readonly User _user;
	private readonly Controller _controller;

	public ControllerPlaylist(ControllerApiPlaylist controllerApiPlaylist, User user, Controller controller)
	{
		_controllerApiPlaylist = controllerApiPlaylist;
		_user = user;
		_controller = controller;
	}


	// get list of user playlists
	public async Task<HashSet<Playlist>> GetUserPlaylists()
	{
		return _user.Playlists ??= await _controllerApiPlaylist.GetUserPlaylistsFromApi();
	}

	// get user playlist (with tracks)
	public async Task<Playlist?> GetUserPlaylist(string playlistId, bool getTracks = false)
	{
		if (GetUserPlaylists() == null)
		{
			return null;
		}

		var playlist = _user.Playlists!.FirstOrDefault(x => x.Id == playlistId);

		if (getTracks)
		{
			var tracks = await _controller.GetPlaylistTracks(playlistId);
		}

		return playlist;
	}
}
