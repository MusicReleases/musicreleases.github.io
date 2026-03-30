namespace JakubKastner.MusicReleases.Spotify.Releases;

internal interface ISpotifyReleaseFilterUrlSynchronizer : IDisposable
{
	Task SetFilterFromUrl(SpotifyReleaseUrlParameters urlParameters);
	Task SetInitFilter();
}