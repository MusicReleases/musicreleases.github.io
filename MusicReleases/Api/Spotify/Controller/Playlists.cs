using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public static partial class Controller
    {
        public static async Task<HashSet<Playlist>> GetPlaylists()
        {
            HashSet<Playlist> playlists = new();
            IList<SimplePlaylist>? playlistsFromApi = await GetPlaylistsApi();

            if (playlistsFromApi == null) return playlists;

            foreach (var playlistApi in playlistsFromApi)
            {
                Playlist playlist = new(simplePlaylist: playlistApi);
                playlists.Add(playlist);
            }

            return playlists;
        }

        private static async Task<IList<SimplePlaylist>?> GetPlaylistsApi()
        {
            if (Main.Client == null) return null;

            var request = new PlaylistCurrentUsersRequest
            {
                Limit = 50
            };

            var response = await Main.Client.Playlists.CurrentUsers(request);
            var playlists = await Main.Client.PaginateAll(response);

            return playlists;
        }
    }
}
