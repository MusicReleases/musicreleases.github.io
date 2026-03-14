namespace JakubKastner.MusicReleases.BackgroundTasks.Services;

internal interface IBackgroundTaskFilterUrlSynchronizer : IDisposable
{
	Task<string> GetInitUrl();
	Task SetFilterFromUrl(string? urlParams, string? searchParam);
}