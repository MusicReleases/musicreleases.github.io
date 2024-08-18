using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiPlaylist(ISpotifyApiClient client, ISpotifyControllerUser controllerUser) : IControllerApiPlaylist
{
	private readonly ISpotifyApiClient _client = client;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	// user playlists
	public async Task<ISet<SpotifyPlaylist>> GetUserPlaylistsFromApi(ISet<SpotifyPlaylist>? existingPlaylists = null)
	{
		var playlistsFromApi = await GetUserPlaylistsApi();
		var playlists = new HashSet<SpotifyPlaylist>();

		if (playlistsFromApi is null)
		{
			return playlists;
		}

		foreach (var playlistApi in playlistsFromApi)
		{
			var existingPlaylist = existingPlaylists?.SingleOrDefault(x => x.Id == playlistApi.Id);

			// keep playlist tracks if snapshot is the same
			var existingPlaylistTracks = new HashSet<string>();
			if (existingPlaylist is not null && playlistApi.SnapshotId == existingPlaylist.SnapshotId)
			{
				existingPlaylistTracks = existingPlaylist.Tracks;
			}

			var currentUserOwned = IsPlaylistOwnedByCurrentUser(playlistApi);
			var playlist = new SpotifyPlaylist(playlistApi, existingPlaylistTracks, currentUserOwned);
			playlists.Add(playlist);

			// TODO get tracks -> existing playlists
		}

		return playlists;
	}

	private async Task<IList<FullPlaylist>?> GetUserPlaylistsApi()
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

	private bool IsPlaylistOwnedByCurrentUser(FullPlaylist playlistApi)
	{
		var playlistOwnerId = playlistApi.Owner?.Id;
		if (string.IsNullOrEmpty(playlistOwnerId))
		{
			return false;
		}
		var user = _controllerUser.GetUser();
		return playlistOwnerId == user?.Info?.Id;
	}


	// playlist tracks
	public async Task<ISet<SpotifyPlaylist>> GetPlaylistsTracksFromApi(ISet<SpotifyPlaylist> playlists, bool forceUpdate)
	{
		foreach (var playlist in playlists)
		{
			playlist.Tracks = await GetPlaylistTracks(playlist, forceUpdate) ?? [];
		}
		return playlists;
	}

	private async Task<HashSet<string>?> GetPlaylistTracks(SpotifyPlaylist playlist, bool forceUpdate)
	{
		if (playlist.Tracks?.Count > 0)
		{
			// TODO force update (before)
			// doesnt need update
			return playlist.Tracks;
		}

		var tracksOrEpisodes = await GetPlaylistTracksApi(playlist.Id);
		if (tracksOrEpisodes is null)
		{
			// 0 tracks
			return [];
		}

		var tracksIds = new HashSet<string>();
		foreach (var trackOrEpisode in tracksOrEpisodes)
		{
			if (trackOrEpisode?.Track is null)
			{
				continue;
			}
			else if (trackOrEpisode.Track is FullTrack track)
			{
				tracksIds.Add(track.Id);
			}
			else if (trackOrEpisode.Track is FullEpisode episode)
			{
				tracksIds.Add(episode.Id);
			}
			else
			{
				throw new NotSupportedException(nameof(trackOrEpisode));
			}
		}
		return tracksIds;
	}

	private async Task<IList<PlaylistTrack<IPlayableItem>>?> GetPlaylistTracksApi(string playlistId)
	{
		var request = new PlaylistGetItemsRequest
		{
			Limit = ApiRequestLimit.PlaylistsTracks,
		};
		request.Fields.Add("items(track(id, type)), next");

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Playlists.GetItems(playlistId, request);
		var tracks = await spotifyClient.PaginateAll(response);

		return tracks;
	}


	// TODO doesnt need right now
	public async Task<IList<SpotifyTrack>> GetPlaylistTracksFromApi(string playlistId)
	{
		var tracks = new List<SpotifyTrack>();

		// TODO : commented
		/*if (_spotifyUser == null) return tracks;
		var playlist = await _spotifyUser.GetPlaylist(playlistId);
		if (playlist != null)
		{
			if (playlist.Tracks.Count > 0) return playlist.Tracks;
		}*/

		// get tracksfrom api
		var tracksFromApi = await GetPlaylistTracksApi(playlistId);
		if (tracksFromApi is null)
		{
			return tracks;
		}

		foreach (var trackApi in tracksFromApi)
		{
			if (trackApi.Track == null) continue;

			// TODO podcasts
			if (trackApi.Track is FullTrack fullTrackApi)
			{
				var track = new SpotifyTrack(fullTrack: fullTrackApi);
				tracks.Add(track);
			}
			else if (trackApi.Track is FullEpisode fullEpisodeApi)
			{
				// podcast (episode)
				var track = new SpotifyTrack(fullEpisode: fullEpisodeApi);
				tracks.Add(track);
			}
			else
			{
				throw new NotSupportedException(nameof(trackApi));
			}
		}

		// TODO : commented
		// save tracks to playlist
		//if (playlist != null) playlist.Tracks = tracks;

		return tracks;
	}
}
