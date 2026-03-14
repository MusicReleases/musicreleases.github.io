namespace JakubKastner.MusicReleases.BackgroundTasks.Objects;

internal interface IBackgroundTaskSubProgressScope : IAsyncDisposable
{
	void Report(double fraction, string? label = null);
}
