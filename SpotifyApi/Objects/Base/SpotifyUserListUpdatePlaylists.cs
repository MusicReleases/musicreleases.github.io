namespace JakubKastner.SpotifyApi.Objects.Base;

public class SpotifyUserListUpdatePlaylists : SpotifyUserListUpdateMain
{
	public DateTime LastUpdateTracks { get; set; }

	public SpotifyUserListUpdatePlaylists(DateTime lastUpdateMain) : base(lastUpdateMain) { }
}
