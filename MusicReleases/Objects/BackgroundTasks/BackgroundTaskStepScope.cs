using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;

namespace JakubKastner.MusicReleases.Objects.BackgroundTasks;

public sealed class BackgroundTaskStepScope(BackgroundTask task, BackgroundTaskStep step, CancellationToken ct, CancellationTokenRegistration? ctr) : IAsyncDisposable
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

		if (_step.Status == BackgroundTaskStatus.Running && (_task.IsCancelRequested || (_ct.CanBeCanceled && _ct.IsCancellationRequested)))
		{
			_step.MarkCanceled();
		}
		else
		{
			_step.MarkFinished();
		}

		/*if (_step.Status == BackgroundTaskStatus.Running)
		{
			if (_task.Status == BackgroundTaskStatus.Canceled)
			{
				_step.MarkCanceled();
			}
			else
			{
				_step.MarkFinished();
			}
		}*/
		/*if (_step.Status == BackgroundTaskStatus.Running)
		{
			if (_task.IsCancelRequested)
			{
				_step.MarkCanceled();
			}
			else
			{
				_step.MarkFinished();
			}
		}*/

		_step.NotifyChange();
		_task.RecalculateProgress();
		_task.NotifyChange();

		return ValueTask.CompletedTask;
	}
}