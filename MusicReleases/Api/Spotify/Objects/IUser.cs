using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify.Objects;

public interface IUser
{
    SortedSet<Artist> Artists { set; }
    PrivateUser? LoggedIn { get; }
    HashSet<Playlist> Playlists { set; }
    Task SetUser();
    Task<SortedSet<Artist>> GetArtists();
    Task<Playlist?> GetPlaylist(string playlistId, bool getTracks = false);
    Task<HashSet<Playlist>> GetPlaylists();
}