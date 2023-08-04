namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUser
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    // TODO country


    public HashSet<SpotifyPlaylist>? Playlists { get; set; }
    public SortedSet<SpotifyArtist>? FollowedArtists { get; set; }
    public SortedSet<SpotifyAlbum> ReleasedAlbums { get; set; } = new();
}
