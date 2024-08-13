namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserList<T> where T : SpotifyIdObject
{
	public ISet<T>? List { get; init; }
	public DateTime LastUpdate { get; init; }

	public SpotifyUserList() { }

	public SpotifyUserList(ISet<T> list, DateTime lastUpdate)
	{
		List = list;
		LastUpdate = lastUpdate;
	}
}