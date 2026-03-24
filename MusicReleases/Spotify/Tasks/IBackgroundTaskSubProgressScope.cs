namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskSubProgressScope : IAsyncDisposable
{
	void Report(double fraction, string? label = null);
}
