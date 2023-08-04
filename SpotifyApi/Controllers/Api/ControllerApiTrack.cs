using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiTrack : IControllerApiTrack
{
	private readonly ISpotifyApiClient _client;

	public ControllerApiTrack(ISpotifyApiClient client)
	{
		_client = client;
	}

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
		if (tracksFromApi == null)
		{
			return tracks;
		}

		foreach (var trackApi in tracksFromApi)
		{
			if (trackApi.Track == null) continue;
			var type = trackApi.Track.Type;
			// TODO podcasts
			if (type == ItemType.Track)
			{
				var fullTrackApi = (FullTrack)trackApi.Track;
				var track = new SpotifyTrack(fullTrack: fullTrackApi);
				tracks.Add(track);
			}
			else
			{
				// podcast (episode)
				var fullEpisodeApi = (FullEpisode)trackApi.Track;
				var track = new SpotifyTrack(fullEpisode: fullEpisodeApi);
				tracks.Add(track);
			}
		}

		// TODO : commented
		// save tracks to playlist
		//if (playlist != null) playlist.Tracks = tracks;

		return tracks;
	}

	private async Task<IList<PlaylistTrack<IPlayableItem>>?> GetPlaylistTracksApi(string playlistId)
	{
		var request = new PlaylistGetItemsRequest
		{
			Limit = ApiRequestLimit.ReleaseTracks,
		};
		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Playlists.GetItems(playlistId, request);
		var tracks = await spotifyClient.PaginateAll(response);
		return tracks;
	}
}