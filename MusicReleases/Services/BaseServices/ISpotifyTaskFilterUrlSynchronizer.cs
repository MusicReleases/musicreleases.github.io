namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyTaskFilterUrlSynchronizer
{
	void Dispose();
	Task<string> GetInitUrl();
	Task SetFilterFromUrl(string? urlParams, string? searchParam);
}