namespace JakubKastner.SpotifyApi.Objects.Base;

public class SpotifyUserList<T, U> where T : SpotifyIdNameUrlObject where U : SpotifyUserListUpdate
{
	public ISet<T>? List { get; init; }
	public U? Update { get; init; }

	public SpotifyUserList() { }

	public SpotifyUserList(ISet<T> list, U update)
	{
		List = list;
		Update = update;
	}
}