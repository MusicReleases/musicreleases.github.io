namespace JakubKastner.MusicReleases.Store.LoaderStore;

public class LoaderAction
{
	public bool Loading { get; }

	public LoaderAction(bool loading)
	{
		Loading = loading;
	}
}
