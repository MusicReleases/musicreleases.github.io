namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserList<T> where T : SpotifyIdNameObject
{
	public ISet<T>? List { get; set; }
	public DateTime LastUpdateMain { get; init; }
	public DateTime LastUpdateSecond { get; init; }

	public SpotifyUserList() { }

	public SpotifyUserList(ISet<T> list, DateTime lastUpdateMain, DateTime? lastUpdateSecond = null)
	{
		List = list;
		LastUpdateMain = lastUpdateMain;
		LastUpdateSecond = lastUpdateSecond ?? LastUpdateSecond;
	}
}