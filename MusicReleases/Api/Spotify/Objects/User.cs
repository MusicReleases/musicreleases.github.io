using MusicReleases.Web.Components.LoggedIn.Menus.Header;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify.Objects
{
    public class User
    {
        /// <summary>
        /// Logged in Spotify user
        /// </summary>
        public PrivateUser LoggedIn { get; private set; }

        public HashSet<Playlist> Playlists
        {
            private get
            {
                return _playlists;
            }
            set
            {
                _playlists = value;
            }
        }
        public SortedSet<Artist> Artists
        {
            private get
            {
                return _artists;
            }
            set
            {
                _artists = value;
            }
        }

        private HashSet<Playlist>? _playlists = null;
        private SortedSet<Artist>? _artists = null;

        public User(PrivateUser user)
        {
            LoggedIn = user;
            //Playlists = new();
        }
        public User(PrivateUser user, HashSet<Playlist> playlists)
        {
            LoggedIn = user;
            _playlists = playlists;
        }

        public async Task<Playlist?> GetPlaylist(string playlistId, bool getTracks = false)
        {
            var playlist = _playlists.Where(x => x.Id == playlistId).FirstOrDefault();

            if (getTracks)
            {
                await Controller.GetTracks(playlistId);
            }

            return playlist;
        }

        public async Task<HashSet<Playlist>> GetPlaylists()
        {
            if (_playlists == null)
            {
                _playlists = await Controller.GetPlaylists();
            }
            return _playlists;
        }
        public async Task<SortedSet<Artist>> GetArtists()
        {
            if (_artists == null)
            {
                _artists = await Controller.GetArtists();
            }
            return _artists;
        }
    }
}
