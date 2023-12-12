namespace JakubKastner.MusicReleases.Store.LoaderStore;

public class LoaderAction(bool loading)
{
	public bool Loading { get; } = loading;
}
