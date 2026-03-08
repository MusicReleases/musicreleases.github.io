namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyTaskFilterUrlSynchronizer
{
	Task<string> GetInitUrl();
	Task SetFilterFromUrl(string? urlParams, string? searchParam);
}