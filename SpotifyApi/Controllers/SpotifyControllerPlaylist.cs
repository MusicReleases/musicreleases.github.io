using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerPlaylist(IControllerApiPlaylist controllerApiPlaylist, IControllerApiTrack controllerApiTrack, ISpotifyControllerUser controllerUser) : ISpotifyControllerPlaylist
{
	private readonly IControllerApiPlaylist _controllerApiPlaylist = controllerApiPlaylist;
	private readonly IControllerApiTrack _controllerApiTrack = controllerApiTrack;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	// get list of user playlists
	public async Task<SpotifyUserList<SpotifyPlaylist>> GetUserPlaylists(bool onlyEditable = false)
	{
		var user = _controllerUser.GetUserRequired();

		if (user.Playlists?.List is null)
		{
			user.Playlists = new(await _controllerApiPlaylist.GetUserPlaylistsFromApi(), DateTime.Now);
		}

		var playlists = user.Playlists;

		if (!onlyEditable)
		{
			return playlists;
		}

		// TODO settings
		var editablePlaylists = playlists.List!.Where(p => p.CurrentUserOwned == true || p.Collaborative == true).ToHashSet();
		playlists = new(editablePlaylists, playlists.LastUpdate);

		return playlists;
	}

	// get user playlist (with tracks)
	public async Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false)
	{
		var playlists = await GetUserPlaylists();

		var playlist = playlists.List.FirstOrDefault(p => p.Id == playlistId);

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
