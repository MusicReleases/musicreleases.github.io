using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public static partial class Controller
    {
        public static async Task<List<Track>> GetTracks(string playlistId)
        {
            List<Track> tracks = new();

            if (Main.User == null) return tracks;

            var playlist = await Main.User.GetPlaylist(playlistId);

            if (playlist != null)
            {
                if (playlist.Tracks.Count > 0) return playlist.Tracks;
            }

            // get tracksfrom api
            IList<PlaylistTrack<IPlayableItem>>? tracksFromApi = await GetTracksApi(playlistId);
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

            // save tracks to playlist
            if (playlist != null) playlist.Tracks = tracks;

            return tracks;
        }

        private static async Task<IList<PlaylistTrack<IPlayableItem>>?> GetTracksApi(string playlistId)
        {
            if (Main.Client == null) return null;

            var request = new PlaylistGetItemsRequest
            {
                Limit = 100
            };

            var tracks = await Main.Client.PaginateAll(await Main.Client.Playlists.GetItems(playlistId,request));
            return tracks;
        }
    }
}
