using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.BackgroundTasks.Objects;

public sealed class BackgroundTaskStep(string name, BackgroundTaskCategory category)
{
	public event Action? OnStateChanged;


	public string Name { get; init; } = name;

	public BackgroundTaskCategory Category { get; init; } = category;

	public Dictionary<string, string> Meta { get; } = [];

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

	// ERRORS
	public string? ErrorCode { get; set; }

	public string? ErrorMessage { get; set; }

	// SUB PROCESS

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

	public int SubprocessSequence { get; set; }

	public DateTimeOffset? LastSubProgressAt { get; set; }


	public bool Ended => _status.HasAnyFlag(BackgroundTaskStatus.Finished, BackgroundTaskStatus.Failed, BackgroundTaskStatus.Canceled);

	public bool IsRunning => !Ended;

	public TimeSpan? Duration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;


	public void NotifyChange()
	{
		OnStateChanged?.Invoke();
	}
}
