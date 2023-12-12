namespace JakubKastner.MusicReleases.Web.Components.InfiniteScrolling;

public sealed class InfiniteScrollingItemsProviderRequest(int startIndex, CancellationToken cancellationToken)
{
	public int StartIndex { get; } = startIndex;

	public CancellationToken CancellationToken { get; } = cancellationToken;
}