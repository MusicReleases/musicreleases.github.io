using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerPlaylist : ISpotifyControllerPlaylist
{
	private readonly IControllerApiPlaylist _controllerApiPlaylist;
	private readonly IControllerApiTrack _controllerApiTrack;
	private readonly SpotifyUser _user;

	public SpotifyControllerPlaylist(IControllerApiPlaylist controllerApiPlaylist, IControllerApiTrack controllerApiTrack, SpotifyUser user)
	{
		_controllerApiPlaylist = controllerApiPlaylist;
		_user = user;
		_controllerApiTrack = controllerApiTrack;
	}

	// get list of user playlists
	public async Task<ISet<SpotifyPlaylist>> GetUserPlaylists(bool onlyEditable = false)
	{
		var playlists = _user.Playlists ??= await _controllerApiPlaylist.GetUserPlaylistsFromApi();

		if (!onlyEditable)
		{
			return new SortedSet<SpotifyPlaylist>(playlists);
		}

		// TODO settings
		return new SortedSet<SpotifyPlaylist>(playlists.Where(playlist => playlist.CurrentUserOwned == true && playlist.Collaborative == false));
	}

	// get user playlist (with tracks)
	public async Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false)
	{
		if (await GetUserPlaylists() == null)
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
