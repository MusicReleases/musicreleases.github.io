using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.SpotifyApi.Objects;

public class User : IUser
{
    /// <summary>
    /// Logged in Spotify user
    /// </sApiUserummary>
    public PrivateUser? LoggedIn { get; private set; }

    private readonly Controller _controller;

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
    private SortedSet<Album>? _albums = null;

    public User(Controller controller)
    {
        _controller = controller;
    }

    public async Task SetUser()
    {
        LoggedIn = await _controller.GetLoggedInUser();
    }

    // get user playlist (with tracks)
    public async Task<Playlist?> GetPlaylist(string playlistId, bool getTracks = false)
    {
        var playlist = _playlists.Where(x => x.Id == playlistId).FirstOrDefault();

        if (getTracks)
        {
            await _controller.GetPlaylistTracks(playlistId);
        }

        return playlist;
    }

    // get list of user playlists
    public async Task<HashSet<Playlist>> GetPlaylists()
    {
        if (_playlists == null)
        {
            _playlists = await _controller.GetUserPlaylists();
        }
        return _playlists;
    }
    // get list of user followed artists
    public async Task<SortedSet<Artist>> GetArtists()
    {
        if (_artists == null)
        {
            _artists = await _controller.GetUserArtists();
        }
        return _artists;
    }

    // get releases
    public async Task<SortedSet<Album>> GetReleases(ReleaseType releaseType = ReleaseType.Albums)
    {
        await GetArtists();

        if (_albums == null)
        {
            _albums = new();

            foreach (var artist in _artists)
            {
                var albumsFromApi = await _controller.GetReleases(releaseType, artist.Id);
                // TODO try again
                if (albumsFromApi == null)
                {
                    albumsFromApi = await _controller.GetReleases(releaseType, artist.Id);
                }
                _albums.UnionWith(albumsFromApi);
            }
        }

        return _albums;
    }
}
