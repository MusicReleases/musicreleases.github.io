using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerPlaylist(IControllerApiPlaylist controllerApiPlaylist, ISpotifyControllerUser controllerUser) : ISpotifyControllerPlaylist
{
	private readonly IControllerApiPlaylist _controllerApiPlaylist = controllerApiPlaylist;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	// get list of user playlists

	public async Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>> GetUserPlaylists(bool onlyEditable = false, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? existingPlaylists = null, bool forceUpdate = false)
	{
		if (existingPlaylists is null)
		{
			forceUpdate = true;
		}
		else
		{
			if (existingPlaylists.Update is null)
			{
				throw new NullReferenceException(nameof(existingPlaylists.Update));
			}

			var dateTimeDifference = DateTime.Now - existingPlaylists.Update.LastUpdateMain;

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

	private async Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>> GetUserPlaylistsApi(bool onlyEditable, bool forceUpdate = false, ISet<SpotifyPlaylist>? existingPlaylists = null)
	{
		var user = _controllerUser.GetUserRequired();

		if (user.Playlists?.List is null || forceUpdate)
		{
			var playlistsApi = await _controllerApiPlaylist.GetUserPlaylistsFromApi(existingPlaylists);
			user.Playlists = new(playlistsApi, new SpotifyUserListUpdatePlaylists(DateTime.Now));
		}

		var playlists = user.Playlists;

		if (!onlyEditable)
		{
			return playlists;
		}

		// TODO settings
		var editablePlaylists = playlists.List!.Where(p => p.CurrentUserOwned == true || p.Collaborative == true).ToHashSet();
		playlists = new SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>(editablePlaylists, new SpotifyUserListUpdatePlaylists(playlists.Update!.LastUpdateMain));

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
			// TODO get tracks
			/*var tracks = await _controllerApiTrack.GetPlaylistTracksFromApi(playlistId);
			playlist.Tracks = [.. tracks];*/
		}

		return playlist;
	}

	public async Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>> GetPlaylistsTracks(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? playlistsStorage = null, bool forceUpdate = false)
	{

		// TODO editable
		var playlists = await GetUserPlaylists(true, playlistsStorage, forceUpdate);
		if (playlists is null)
		{
			// 0 playlists
			throw new NullReferenceException(nameof(playlists));
		}
		if (playlists.List is null)
		{
			// 0 playlists
			return playlists;
		}


		if (playlists.Update is not SpotifyUserListUpdatePlaylists)
		{
			throw new NotSupportedException(nameof(playlists.Update));
		}
		var lastUpdateList = (playlists.Update as SpotifyUserListUpdatePlaylists)!;

		var dateTimeDifference = DateTime.Now - lastUpdateList.LastUpdateTracks;

		if (dateTimeDifference.TotalHours >= 24)
		{
			// force update every 24 hours
			forceUpdate = true;
		}


		if (!forceUpdate)
		{
			var existingPlaylistsWithTracks = playlists.List!.Any(x => x.Tracks.Count > 0);
			if (!existingPlaylistsWithTracks)
			{
				forceUpdate = true;
			}
		}

		if (!forceUpdate)
		{
			// doesnt need update

			// TODO editable playlists switch ???
			return playlists;
		}

		var playlistsWithTracks = await GetPlaylistsTracksApi(playlists.List, forceUpdate, lastUpdateList);
		return playlistsWithTracks;
	}

	private async Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>> GetPlaylistsTracksApi(ISet<SpotifyPlaylist> playlistsSaved, bool forceUpdate, SpotifyUserListUpdatePlaylists lastUpdate)
	{
		var playlists = await _controllerApiPlaylist.GetPlaylistsTracksFromApi(playlistsSaved, forceUpdate);
		lastUpdate.LastUpdateTracks = DateTime.Now;
		var playlistStorage = new SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>(playlists, lastUpdate);
		return playlistStorage;
	}

}
