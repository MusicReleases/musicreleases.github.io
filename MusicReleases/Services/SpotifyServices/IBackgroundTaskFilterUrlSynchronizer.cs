namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public interface IBackgroundTaskFilterUrlSynchronizer
{
	Task<string> GetInitUrl();
	Task SetFilterFromUrl(string? urlParams, string? searchParam);
}