namespace JakubKastner.MusicReleases.Objects.BackgroundTasks;

public interface IBackgroundTaskSubProgressScope : IAsyncDisposable
{
	void Report(double fraction, string? label = null);
}
