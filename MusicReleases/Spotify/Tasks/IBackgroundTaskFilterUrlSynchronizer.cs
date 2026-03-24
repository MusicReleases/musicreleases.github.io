namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskFilterUrlSynchronizer : IDisposable
{
	Task<string> GetInitUrl();
	Task SetFilterFromUrl(string? urlParams, string? searchParam);
}