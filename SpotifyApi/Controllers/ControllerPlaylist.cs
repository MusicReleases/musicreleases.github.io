using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class ControllerPlaylist
{
	private readonly ControllerApiPlaylist _controllerApiPlaylist;
	private readonly ControllerApiTrack _controllerApiTrack;
	private readonly User _user;

	public ControllerPlaylist(ControllerApiPlaylist controllerApiPlaylist, ControllerApiTrack controllerApiTrack, User user)
	{
		_controllerApiPlaylist = controllerApiPlaylist;
		_user = user;
		_controllerApiTrack = controllerApiTrack;
	}

	// get list of user playlists
	public async Task<HashSet<Playlist>> GetAllUserPlaylists()
	{
		return _user.Playlists ??= await _controllerApiPlaylist.GetUserPlaylistsFromApi();
	}

	// get user playlist (with tracks)
	public async Task<Playlist?> GetUserPlaylist(string playlistId, bool getTracks = false)
	{
		if (await GetAllUserPlaylists() == null)
		{
			// TODO getalluserplaylist() is not nullable
			return null;
		}

		var playlist = _user.Playlists!.FirstOrDefault(x => x.Id == playlistId);

		if (getTracks)
		{
			var tracks = await _controllerApiTrack.GetPlaylistTracksFromApi(playlistId);
		}

		return playlist;
	}
}
