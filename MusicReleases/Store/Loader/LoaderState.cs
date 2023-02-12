namespace JakubKastner.MusicReleases.Store.Releases;

public record LoaderState
{
	public bool Loading { get; init; }
	public string LoadingClass() => Loading ? " active" : string.Empty;
}
