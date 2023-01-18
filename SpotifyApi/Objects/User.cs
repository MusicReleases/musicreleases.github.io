namespace JakubKastner.SpotifyApi.Objects;

public class User
{
	public HashSet<Playlist>? Playlists { get; set; }
	public SortedSet<Artist>? FollowedArtists { get; set; }
	public SortedSet<Album> ReleasedAlbums { get; set; } = new();
}
