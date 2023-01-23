using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiTrack
{
    private readonly Client _client;

    public ControllerApiTrack(Client client)
    {
        _client = client;
    }

    public async Task<List<Track>> GetPlaylistTracksFromApi(string playlistId)
    {
        List<Track> tracks = new();

        // TODO : commented
        /*if (_spotifyUser == null) return tracks;
        var playlist = await _spotifyUser.GetPlaylist(playlistId);
        if (playlist != null)
        {
            if (playlist.Tracks.Count > 0) return playlist.Tracks;
        }*/

        // get tracksfrom api
        var tracksFromApi = await GetPlaylistTracksApi(playlistId);
        if (tracksFromApi == null) return tracks;

        foreach (var trackApi in tracksFromApi)
        {
            if (trackApi.Track == null) continue;
            var type = trackApi.Track.Type;
            // TODO podcasts
            if (type == ItemType.Track)
            {
                var fullTrackApi = (FullTrack)trackApi.Track;
                Track track = new(fullTrack: fullTrackApi);
                tracks.Add(track);
            }
            else
            {
                // podcast (episode)
                var fullEpisodeApi = (FullEpisode)trackApi.Track;
                Track track = new(fullEpisode: fullEpisodeApi);
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