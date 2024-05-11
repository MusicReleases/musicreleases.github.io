namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserList<T> where T : SpotifyIdObject
{
	public ISet<T>? List { get; private set; }
	public DateTime LastUpdate { get; set; }

	public SpotifyUserList() { }

	public SpotifyUserList(ISet<T> list, DateTime lastUpdate)
	{
		List = list;
		LastUpdate = lastUpdate;
	}
}