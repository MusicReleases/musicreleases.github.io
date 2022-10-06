using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify.Objects;

public class User : IUser
{
    /// <summary>
    /// Logged in Spotify user
    /// </sApiUserummary>
    public PrivateUser? ApiUser { get; private set; }
    //public ISpotifyClient? Client { get; private set; }

    private Controller _controller;

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

    public User(Controller controller)
    {
        _controller = controller;
    }
    /*public User(PrivateUser user)
    {
        LoggedIn = user;
        //Playlists = new();
    }*/
    /*public User(PrivateUser user, HashSet<Playlist> playlists)
    {
        LoggedIn = user;
        _playlists = playlists;
    }*/

    /*public async Task SetUser(string url)
    {
        await SetUser(new Uri(url));
    }

    public async Task SetUser(Uri url)
    {
        // get url parameters
        var maxLen = Math.Min(1, url.Fragment.Length);
        Dictionary<string, string> urlParameters = url.Fragment[maxLen..]?
          .Split("&", StringSplitOptions.RemoveEmptyEntries)?
          .Select(param => param.Split("=", StringSplitOptions.RemoveEmptyEntries))?
          .ToDictionary(param => param[0], param => param[1]) ?? new Dictionary<string, string>();

        // get user from access token
        var loggedIn = urlParameters.ContainsKey("access_token");
        if (!loggedIn) return;

        var accessToken = urlParameters["access_token"];
        Client = new SpotifyClient(accessToken);

        ApiUser = await Client.UserProfile.Current();

        if (!urlParameters.ContainsKey("expires_in")) return;
        var accessTokenExpires = urlParameters["expires_in"];
    }

    public void SetUser(PrivateUser user)
    {
        ApiUser = user;
    }*/

    public async Task SetUser()
    {
        ApiUser = await _controller.GetLoggedInUser();
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

    // get user playlists
    public async Task<HashSet<Playlist>> GetPlaylists()
    {
        if (_playlists == null)
        {
            _playlists = await _controller.GetUserPlaylists();
        }
        return _playlists;
    }
    // get user followed artists
    public async Task<SortedSet<Artist>> GetArtists()
    {
        if (_artists == null)
        {
            _artists = await _controller.GetUserArtists();
        }
        return _artists;
    }
}
