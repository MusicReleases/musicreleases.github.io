namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public interface ISpotifyTaskFilterUrlSynchronizer
{
	Task<string> GetInitUrl();
	Task SetFilterFromUrl(string? urlParams, string? searchParam);
}