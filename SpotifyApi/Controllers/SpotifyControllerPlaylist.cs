using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerPlaylist(IControllerApiPlaylist controllerApiPlaylist, IControllerApiTrack controllerApiTrack, ISpotifyControllerUser controllerUser) : ISpotifyControllerPlaylist
{
	private readonly IControllerApiPlaylist _controllerApiPlaylist = controllerApiPlaylist;
	private readonly IControllerApiTrack _controllerApiTrack = controllerApiTrack;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	// get list of user playlists
	public async Task<ISet<SpotifyPlaylist>> GetUserPlaylists(bool onlyEditable = false)
	{
		var user = _controllerUser.GetUserRequired();
		var playlists = new SortedSet<SpotifyPlaylist>(user.Playlists ??= await _controllerApiPlaylist.GetUserPlaylistsFromApi());

		if (!onlyEditable)
		{
			return playlists;
		}

		// TODO settings
		playlists = new(playlists.Where(p => p.CurrentUserOwned == true || p.Collaborative == true));

		return playlists;
	}

	// get user playlist (with tracks)
	public async Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false)
	{
		var playlists = await GetUserPlaylists();

		var playlist = playlists.FirstOrDefault(p => p.Id == playlistId);

		if (playlist is null)
		{
			throw new NullReferenceException(nameof(playlist));
		}

		if (getTracks)
		{
			var tracks = await _controllerApiTrack.GetPlaylistTracksFromApi(playlistId);
			playlist.Tracks = [.. tracks];
		}

		return playlist;
	}
}
