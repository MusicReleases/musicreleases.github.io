using JakubKastner.MusicReleases.BackgroundTasks.Extensions;

namespace JakubKastner.MusicReleases.BackgroundTasks.Objects;

internal sealed class BackgroundTaskStepScope(BackgroundTask task, BackgroundTaskStep step, CancellationToken ct, CancellationTokenRegistration? ctr) : IAsyncDisposable
{
	private readonly BackgroundTask _task = task;
	private readonly BackgroundTaskStep _step = step;
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