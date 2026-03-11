using JakubKastner.MusicReleases.Services.BaseServices;
using System.Globalization;

namespace JakubKastner.MusicReleases.Objects.BackgroundTasks;

public class BackgroundTaskSubProgressScope(BackgroundTask task, BackgroundTaskStep step, double from, double to, string segmentLabel) : IBackgroundTaskSubProgressScope
{
	private readonly BackgroundTask _task = task;
	private readonly BackgroundTaskStep _step = step;
	private readonly double _from = from;
	private readonly double _to = to;
	private readonly string _segmentLabel = segmentLabel;
	private readonly DateTimeOffset _startedAt = DateTimeOffset.UtcNow;

	private bool _disposed;

	public void Report(double fraction, string? label = null)
	{
		if (_disposed)
		{
			return;
		}

		fraction = Math.Clamp(fraction, 0, 1);
		var mapped = _from + ((_to - _from) * fraction);
		_task.SetSubProgress(mapped, label ?? $"{_segmentLabel}:{(int)(fraction * 100)}%");
	}

	public ValueTask DisposeAsync()
	{
		if (_disposed)
		{
			return ValueTask.CompletedTask;
		}
		_disposed = true;

		var duration = DateTimeOffset.UtcNow - _startedAt;

		var seq = _step.SubSeq.ToString("D2", CultureInfo.InvariantCulture);
		_step.Meta[$"seg.{seq}.label"] = _segmentLabel;
		_step.Meta[$"seg.{seq}.duration.ms"] = duration.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

		_task.SetSubProgress(_to, _segmentLabel + ":end");

		return ValueTask.CompletedTask;
	}
}
