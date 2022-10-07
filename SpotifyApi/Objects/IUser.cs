using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.SpotifyApi.Objects;

public interface IUser
{
    SortedSet<Artist> Artists { set; }
    PrivateUser? LoggedIn { get; }
    HashSet<Playlist> Playlists { set; }
    Task SetUser();
    Task<SortedSet<Artist>> GetArtists();
    Task<Playlist?> GetPlaylist(string playlistId, bool getTracks = false);
    Task<HashSet<Playlist>> GetPlaylists();
    Task<SortedSet<Album>> GetReleases(ReleaseType releaseType = ReleaseType.Albums);
}