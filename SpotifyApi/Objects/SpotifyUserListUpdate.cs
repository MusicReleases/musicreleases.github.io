namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserListUpdate
{
	public DateTime LastUpdateMain { get; init; }
	public SpotifyUserListUpdate(DateTime lastUpdateMain)
	{
		LastUpdateMain = lastUpdateMain;
	}
}
