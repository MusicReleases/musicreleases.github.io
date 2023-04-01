namespace JakubKastner.MusicReleases.Web.Components.InfiniteScrolling;

public sealed class InfiniteScrollingItemsProviderRequest
{
    public int StartIndex { get; }

    public CancellationToken CancellationToken { get; }

    public InfiniteScrollingItemsProviderRequest(int startIndex, CancellationToken cancellationToken)
    {
        StartIndex = startIndex;
        CancellationToken = cancellationToken;
    }
}