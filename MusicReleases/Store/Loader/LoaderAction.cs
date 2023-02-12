namespace JakubKastner.MusicReleases.Store.Releases;

public class LoaderAction
{
	public bool Loading { get; }

	public LoaderAction(bool loading)
	{
		Loading = loading;
	}
}
