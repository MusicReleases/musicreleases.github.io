using System.Globalization;

namespace JakubKastner.MusicReleases.Spotify.Tasks;

public sealed class BackgroundTaskStep
{
	public event Action? OnStateChanged;

	internal CancellationToken TaskCt { get; set; }

	public Guid StepId { get; init; } = Guid.NewGuid();
	public string Name { get; init; } = string.Empty;
	public BackgroundTaskCategory Category { get; init; }

	public BackgroundTaskStatus Status { get; private set; } = BackgroundTaskStatus.Running;
	public BackgroundStepOutcome Outcome { get; private set; } = BackgroundStepOutcome.Executed;

	public bool IsWaiting { get; internal set; }
	public Guid? WaitingForStepId { get; internal set; }

	public string? SkipReason { get; private set; }
	public string? ErrorCode { get; private set; }
	public string? ErrorMessage { get; private set; }

	public DateTimeOffset StartedAt { get; private set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset? FinishedAt { get; private set; }

	public TimeSpan? Duration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;

	public Dictionary<string, string> Meta { get; } = new();

	public double SubProgress { get; private set; }
	public int SubprocessSequence { get; private set; }
	public DateTimeOffset? LastSubProgressAt { get; private set; }

	private bool _autoSegmentCount;
	private int _implicitSegments;

	private int SegmentCount { get; set; } = 1;
	private int SegmentIndex { get; set; } = 0;

	public bool Ended => Status is BackgroundTaskStatus.Finished or BackgroundTaskStatus.Failed or BackgroundTaskStatus.Canceled;

	public void NotifyChange()
	{
		if (Outcome == BackgroundStepOutcome.Skipped)
		{
			return;
		}

		OnStateChanged?.Invoke();
	}

	public void MarkSkipped(string reason)
	{
		if (Ended)
		{
			return;
		}

		Outcome = BackgroundStepOutcome.Skipped;
		SkipReason = reason;
		SubProgress = 1.0;
		Status = BackgroundTaskStatus.Finished;
		FinishedAt = DateTimeOffset.UtcNow;
		//NotifyChange();
	}

	public void MarkFinished()
	{
		if (Ended)
		{
			return;
		}

		SubProgress = 1.0;
		Status = BackgroundTaskStatus.Finished;
		FinishedAt = DateTimeOffset.UtcNow;
		NotifyChange();
	}

	public void MarkCanceled()
	{
		if (Ended)
		{
			return;
		}

		Status = BackgroundTaskStatus.Canceled;
		FinishedAt = DateTimeOffset.UtcNow;
		NotifyChange();
	}

	public void MarkFailed(Exception ex, string? code = null)
	{
		if (Ended)
		{
			return;
		}

		Status = BackgroundTaskStatus.Failed;
		FinishedAt = DateTimeOffset.UtcNow;
		ErrorMessage = ex.Message;
		ErrorCode = code ?? "ERR_STEP";
		NotifyChange();
	}

	// ----------------------------
	// Segments (owned by Step)
	// ----------------------------

	public void BeginAutoSegments(int count)
	{
		_autoSegmentCount = true;
		SegmentCount = Math.Max(1, count);
		SegmentIndex = 0;

		Meta["seg.count"] = SegmentCount.ToString(CultureInfo.InvariantCulture);
		Meta["seg.index"] = SegmentIndex.ToString(CultureInfo.InvariantCulture);

		NotifyChange();
	}

	public void SetSubProgress(double value, string? label = null)
	{
		value = Math.Clamp(value, 0, 1);

		var now = DateTimeOffset.UtcNow;
		var last = LastSubProgressAt ?? StartedAt;
		var delta = now - last;

		SubProgress = value;
		LastSubProgressAt = now;
		SubprocessSequence++;

		var seq = SubprocessSequence.ToString("D2", CultureInfo.InvariantCulture);

		if (!string.IsNullOrWhiteSpace(label))
		{
			Meta[$"sub.{seq}.label"] = label;
		}

		Meta[$"sub.{seq}.ms"] = delta.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
		NotifyChange();
	}

	public async Task<T> RunSegment<T>(string label, Func<CancellationToken, Task<T>> body)
	{
		await using var _ = BeginSegment(label);
		return await body(TaskCt);
	}

	public async Task RunSegment(string label, Func<CancellationToken, Task> body)
	{
		await using var _ = BeginSegment(label);
		await body(TaskCt);
	}

	private IAsyncDisposable BeginSegment(string label)
	{
		if (!_autoSegmentCount)
		{
			_implicitSegments++;
			SegmentCount = _implicitSegments;
		}


		var count = Math.Max(1, SegmentCount);
		var index = SegmentIndex;

		var segmentSize = 1.0 / count;
		var from = index * segmentSize;
		var to = from + segmentSize;

		SegmentIndex = Math.Min(index + 1, count);
		Meta["seg.index"] = SegmentIndex.ToString(CultureInfo.InvariantCulture);

		SetSubProgress(from, $"{label}:begin");
		return new SegmentScope(this, to, label);
	}

	private sealed class SegmentScope : IAsyncDisposable
	{
		private readonly BackgroundTaskStep _step;
		private readonly double _to;
		private readonly string _label;

		private bool _disposed;

		public SegmentScope(BackgroundTaskStep step, double to, string label)
		{
			_step = step;
			_to = to;
			_label = label;
		}

		public ValueTask DisposeAsync()
		{
			if (_disposed)
			{
				return ValueTask.CompletedTask;
			}

			_disposed = true;
			_step.SetSubProgress(_to, $"{_label}:end");
			return ValueTask.CompletedTask;
		}
	}
}