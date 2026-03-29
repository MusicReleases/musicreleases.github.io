namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class BackgroundTaskStepScope(BackgroundTask2 task, BackgroundTaskStep2 step, CancellationToken ct, CancellationTokenRegistration? ctr) : IAsyncDisposable
{
	private readonly BackgroundTask2 _task = task;
	private readonly BackgroundTaskStep2 _step = step;
	private readonly CancellationToken _ct = ct;
	private readonly CancellationTokenRegistration? _ctr = ctr;

	private bool _disposed;

	public ValueTask DisposeAsync()
	{
		if (_disposed)
		{
			return ValueTask.CompletedTask;
		}
		_disposed = true;
		_ctr?.Dispose();


		if (_step.IsRunning && (_task.IsCancelRequested || (_ct.CanBeCanceled && _ct.IsCancellationRequested)))
		{
			_step.MarkCanceled();
			_step.NotifyChange();
		}

		//_step.NotifyChange();
		_task.RecalculateProgress();
		_task.NotifyChange();

		return ValueTask.CompletedTask;
	}
}