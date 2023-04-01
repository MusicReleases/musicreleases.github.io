namespace JakubKastner.MusicReleases.Web.Components.InfiniteScrolling;

public delegate Task<IEnumerable<T>> InfiniteScrollingItemsProviderRequestDelegate<T>(InfiniteScrollingItemsProviderRequest context);