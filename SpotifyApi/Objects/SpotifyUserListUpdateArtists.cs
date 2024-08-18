namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserListUpdateArtists : SpotifyUserListUpdate
{

	public DateTime LastUpdateAlbums { get; set; }
	public DateTime LastUpdateTracks { get; set; }
	public DateTime LastUpdateAppears { get; set; }
	public DateTime LastUpdateCompilations { get; set; }
	public DateTime LastUpdatePodcasts { get; set; }

	public SpotifyUserListUpdateArtists(DateTime lastUpdateMain) : base(lastUpdateMain) { }
}
