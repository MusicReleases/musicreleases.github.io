namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyTaskFilterUrlSynchronizer
{
	void Dispose();
	void SetFilterFromUrl(string? urlParams, string? searchParam);
}