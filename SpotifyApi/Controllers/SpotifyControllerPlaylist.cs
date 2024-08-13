using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerPlaylist(IControllerApiPlaylist controllerApiPlaylist, IControllerApiTrack controllerApiTrack, ISpotifyControllerUser controllerUser) : ISpotifyControllerPlaylist
{
	private readonly IControllerApiPlaylist _controllerApiPlaylist = controllerApiPlaylist;
	private readonly IControllerApiTrack _controllerApiTrack = controllerApiTrack;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	// get list of user playlists

	private async Task<SpotifyUserList<SpotifyPlaylist>> GetUserPlaylistsApi(bool onlyEditable, bool forceUpdate = false, ISet<SpotifyPlaylist>? existingPlaylists = null)
	{
		var user = _controllerUser.GetUserRequired();

		if (user.Playlists?.List is null || forceUpdate)
		{
			var playlistsApi = await _controllerApiPlaylist.GetUserPlaylistsFromApi(existingPlaylists);
			user.Playlists = new(playlistsApi, DateTime.Now);
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

	public async Task<SpotifyUserList<SpotifyPlaylist>> GetUserPlaylists(bool onlyEditable = false, SpotifyUserList<SpotifyPlaylist>? existingPlaylists = null, bool forceUpdate = false)
	{
		if (existingPlaylists is null)
		{
			forceUpdate = true;
		}
		else
		{
			var dateTimeDifference = DateTime.Now - existingPlaylists.LastUpdate;

			if (dateTimeDifference.TotalHours >= 24)
			{
				// force update every 24 hours
				forceUpdate = true;
			}
		}

		if (!forceUpdate)
		{
			// doesnt need update

			// TODO editable playlists switch ???
			return existingPlaylists!;
		}

		var playlists = await GetUserPlaylistsApi(onlyEditable, forceUpdate, existingPlaylists?.List);
		return playlists;
	}

	// get user playlist (with tracks)
	public async Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false)
	{
		var playlists = await GetUserPlaylists();

		var playlist = playlists.List?.FirstOrDefault(p => p.Id == playlistId);

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
