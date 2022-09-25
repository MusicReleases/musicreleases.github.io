using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public partial class Controller
    {
        public async Task<HashSet<Playlist>> GetPlaylists()
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

        private async Task<IList<SimplePlaylist>?> GetPlaylistsApi()
        {
            if (_spotifyUser.Client == null) return null;

            var request = new PlaylistCurrentUsersRequest
            {
                Limit = 50
            };

            var response = await _spotifyUser.Client.Playlists.CurrentUsers(request);
            var playlists = await _spotifyUser.Client.PaginateAll(response);

            return playlists;
        }
    }
}
