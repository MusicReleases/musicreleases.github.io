using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify.Objects
{
    public interface IUser
    {
        SortedSet<Artist> Artists { set; }
        ISpotifyClient? Client { get; }
        PrivateUser? LoggedIn { get; }
        HashSet<Playlist> Playlists { set; }

        Task<SortedSet<Artist>> GetArtists();
        Task<Playlist?> GetPlaylist(string playlistId, bool getTracks = false);
        Task<HashSet<Playlist>> GetPlaylists();
        void SetUser(PrivateUser user);
        Task SetUser(string url);
        Task SetUser(Uri url);
    }
}