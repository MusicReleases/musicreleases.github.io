namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUser
{
	public string? Id { get; set; }
	public string? Name { get; set; }
	// TODO country


	public ISet<SpotifyPlaylist>? Playlists { get; set; }
	public ISet<SpotifyArtist>? FollowedArtists { get; set; }
	public ISet<SpotifyAlbum> ReleasedAlbums { get; set; } = new SortedSet<SpotifyAlbum>();
}
