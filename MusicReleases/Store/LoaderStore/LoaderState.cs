namespace JakubKastner.MusicReleases.Store.LoaderStore;

public record LoaderState
{
	public bool Loading { get; init; }
	public string LoadingClass => Loading ? "active" : string.Empty;
}
