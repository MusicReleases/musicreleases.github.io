using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Objects.BackgroundTasks;

public class BackgroundTask(BackgroundTaskType type, string name, string info)
{
	public event Action? OnStateChanged;

	public Guid Id { get; } = Guid.NewGuid();

	public string Name { get; init; } = name;

	public string Info { get; init; } = info;

	public BackgroundTaskType Type { get; init; } = type;


	public DateTimeOffset? StartedAt => _steps.FirstOrDefault()?.StartedAt;

	public DateTimeOffset? FinishedAt => _steps.LastOrDefault(s => s.FinishedAt.HasValue)?.FinishedAt;


	public TimeSpan? Duration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;


	private string? _statusText = null;
	public string StatusText
	{
		get
		{
			var text = Status switch
			{
				BackgroundTaskStatus.Running => "Running",
				BackgroundTaskStatus.Finished => "Finished",
				BackgroundTaskStatus.Failed => "Failed",
				BackgroundTaskStatus.Canceled => "Canceled by user",
				_ => "Unknown"
			};

			if (_statusText.IsNotNullOrEmpty())
			{
				text += $": {_statusText}";
			}

			return text;
		}
	}
	/*public string StatusText
	{
		get => _statusText;
		set
		{
			if (_statusText != value)
			{
				_statusText = value;
				NotifyChange();
			}
		}
	}*/
	public bool IsCancelRequested { get; internal set; }

	//public BackgroundTaskStatus Status { get; private set; } = BackgroundTaskStatus.Running;

	public BackgroundTaskStatus Status
	{
		get
		{
			if (_steps.Count == 0)
			{
				return BackgroundTaskStatus.Finished;
			}

			if (_steps.Any(s => s.Status == BackgroundTaskStatus.Failed))
			{
				return BackgroundTaskStatus.Failed;
			}

			if (_steps.Any(s => s.Status == BackgroundTaskStatus.Canceled))
			{
				return BackgroundTaskStatus.Canceled;
			}

			if (_steps.All(s => s.Status == BackgroundTaskStatus.Finished))
			{
				return BackgroundTaskStatus.Finished;
			}

			return BackgroundTaskStatus.Running;
		}
	}

	public bool Ended => Status.HasAnyFlag(BackgroundTaskStatus.Finished, BackgroundTaskStatus.Failed, BackgroundTaskStatus.Canceled);
	public bool IsRunning => !Ended;
	public bool Failed => Status == BackgroundTaskStatus.Failed;

	private double _progress;
	public double Progress
	{
		get => _progress;
		set
		{
			if (Math.Abs(_progress - value) > 0.001)
			{
				_progress = value;
				NotifyChange();
			}
		}
	}

	private bool _isOverlayVisible = true;

	public bool IsOverlayVisible
	{
		get => _isOverlayVisible;
		set
		{
			if (_isOverlayVisible != value)
			{
				_isOverlayVisible = value;
				NotifyChange();
			}
		}
	}

	public int CurrentStepIndex { get; private set; }

	public BackgroundTaskStep? CurrentStep => Steps.ElementAtOrDefault(CurrentStepIndex);

	public IReadOnlyList<BackgroundTaskStep> Steps => _steps;
	private readonly List<BackgroundTaskStep> _steps = [];

	public IReadOnlyList<BackgroundTaskLink> Links => _links;
	private readonly List<BackgroundTaskLink> _links = [];


	public CancellationTokenSource Cts { get; } = new();
	public CancellationToken Token => Cts.Token;

	public void NotifyChange()
	{
		OnStateChanged?.Invoke();
	}

	private void HandleStepChanged()
	{
		RecalculateProgress();
		NotifyChange();
	}

	public void AddStep(BackgroundTaskStep step)
	{
		_steps.Add(step);
		CurrentStepIndex = Steps.Count - 1;

		step.OnStateChanged += HandleStepChanged;
		RecalculateProgress();
		NotifyChange();
	}

	public void RecalculateProgress()
	{
		if (Steps.Count == 0)
		{
			Progress = 0;
			return;
		}

		var idx = CurrentStepIndex;
		var total = Steps.Count;

		var basePart = Math.Clamp((double)idx / total, 0, 1);

		var step = Steps[idx];
		var sub = Math.Clamp(step.SubProgress, 0, 1) / total;

		Progress = Math.Clamp(basePart + sub, 0, 1);
	}

	public void AddLink(string text, string title, string urlWeb, Enum icon)
	{
		var link = new BackgroundTaskLink(text, title, null, urlWeb, icon);
		_links.Add(link);
		NotifyChange();
	}

	public void AddLink(string text, string title, SpotifyIdNameUrlObject spotifyUrlObject, Enum? icon = null)
	{
		icon ??= SpotifyIcon.SmallGreen;

		var link = new BackgroundTaskLink(text, title, spotifyUrlObject.UrlApp, spotifyUrlObject.UrlWeb, icon);
		_links.Add(link);
		NotifyChange();
	}



	public void RequestCancel()
	{
		if (!IsCancelRequested)
		{
			IsCancelRequested = true;
			try
			{
				Cts.Cancel();
			}
			catch
			{
				// ignore
			}
			NotifyChange();
		}
	}


	public void MarkCanceled()
	{
		IsCancelRequested = true;
		//Status = BackgroundTaskStatus.Canceled;
		//_statusText = "Canceled by user";

		foreach (var step in Steps.Where(s => s.Status == BackgroundTaskStatus.Running))
		{
			step.MarkCanceled();
		}

		NotifyChange();
	}

	public void MarkFailed(Exception ex)
	{
		//Status = BackgroundTaskStatus.Failed;
		_statusText = ex.Message;

		foreach (var step in Steps.Where(s => s.Status == BackgroundTaskStatus.Running))
		{
			step.MarkFailed(ex);
		}

		NotifyChange();
	}
	public void MarkFinished()
	{
		if (Ended || IsCancelRequested)
		{
			return;
		}

		//Status = BackgroundTaskStatus.Finished;
		//_statusText = "Finished";

		foreach (var step in Steps.Where(s => s.Status == BackgroundTaskStatus.Running))
		{
			step.MarkFinished();
		}
		NotifyChange();
	}
}
