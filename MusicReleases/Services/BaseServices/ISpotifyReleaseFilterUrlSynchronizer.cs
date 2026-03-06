namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ISpotifyReleaseFilterUrlSynchronizer
	{
		void Dispose();
		Task SetFilterFromUrl(string? releaseType, string? year, string? month, string? artist, string? advancedFilterParams, string? searchParam);
		Task SetInitFilter();
	}
}