namespace JakubKastner.MusicReleases.BackgroundTasks.Objects;

internal sealed class BackgroundTaskSubProgressScopeNoop : IBackgroundTaskSubProgressScope
{
	public void Report(double fraction, string? label = null)
	{
	}

	public ValueTask DisposeAsync()
	{
		GC.SuppressFinalize(this);
		return ValueTask.CompletedTask;
	}
}
