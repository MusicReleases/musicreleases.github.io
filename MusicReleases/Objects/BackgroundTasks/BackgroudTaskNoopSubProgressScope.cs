namespace JakubKastner.MusicReleases.Objects.BackgroundTasks;

public class BackgroudTaskNoopSubProgressScope : IBackgroundTaskSubProgressScope
{
	public void Report(double fraction, string? label = null) { }
	public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
