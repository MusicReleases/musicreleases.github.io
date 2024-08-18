namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserListUpdatePlaylists : SpotifyUserListUpdate
{
	public DateTime LastUpdateTracks { get; set; }

	public SpotifyUserListUpdatePlaylists(DateTime lastUpdateMain) : base(lastUpdateMain) { }
}
