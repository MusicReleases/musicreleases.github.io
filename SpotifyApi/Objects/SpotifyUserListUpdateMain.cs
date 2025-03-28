namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserListUpdateMain : SpotifyUserListUpdate
{
	public DateTime LastUpdateMain { get; init; }
	public SpotifyUserListUpdateMain(DateTime lastUpdateMain)
	{
		LastUpdateMain = lastUpdateMain;
	}
}
