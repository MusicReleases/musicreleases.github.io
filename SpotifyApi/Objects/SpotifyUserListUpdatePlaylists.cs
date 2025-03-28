namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserListUpdatePlaylists : SpotifyUserListUpdateMain
{
	public DateTime LastUpdateTracks { get; set; }

	public SpotifyUserListUpdatePlaylists(DateTime lastUpdateMain) : base(lastUpdateMain) { }
}
