using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Objects.BackgroundTasks;

public sealed class BackgroundTaskStep(string name, BackgroundTaskCategory category)
{
	public event Action? OnStateChanged;

	public string Name { get; init; } = name;
	public BackgroundTaskCategory Category { get; init; } = category;

	private BackgroundTaskStatus _status = BackgroundTaskStatus.Running;
	public BackgroundTaskStatus Status
	{
		get => _status;
		set
		{
			if (_status != value)
			{
				_status = value;
				NotifyChange();
			}
		}
	}

	public bool Ended => _status.HasAnyFlag(BackgroundTaskStatus.Finished, BackgroundTaskStatus.Failed, BackgroundTaskStatus.Canceled);
	public bool IsRunning => !Ended;

	private DateTimeOffset _startedAt = DateTimeOffset.UtcNow;
	public DateTimeOffset StartedAt
	{
		get => _startedAt;
		set
		{
			if (_startedAt != value)
			{
				_startedAt = value;
				NotifyChange();
			}
		}
	}

	private DateTimeOffset? _finishedAt;
	public DateTimeOffset? FinishedAt
	{
		get => _finishedAt;
		set
		{
			if (_finishedAt != value)
			{
				_finishedAt = value;
				NotifyChange();
			}
		}
	}

	public TimeSpan? Duration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;

	public int Attempt { get; set; } = 1;
	public string? ErrorCode { get; set; }
	public string? ErrorMessage { get; set; }
	public bool Transient { get; set; }

	public Dictionary<string, string> Meta { get; } = [];

	private double _subProgress;
	public double SubProgress
	{
		get => _subProgress;
		set
		{
			var clamped = Math.Clamp(value, 0, 1);
			if (Math.Abs(_subProgress - clamped) > 0.001)
			{
				_subProgress = clamped;
				NotifyChange();
			}
		}
	}

	public void NotifyChange()
	{
		OnStateChanged?.Invoke();
	}

	public DateTimeOffset? LastSubProgressAt { get; set; }

	public int SubSeq { get; set; }
}
