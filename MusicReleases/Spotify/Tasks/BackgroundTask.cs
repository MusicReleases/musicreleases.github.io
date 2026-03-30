using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Base.Objects;

namespace JakubKastner.MusicReleases.Spotify.Tasks;

public sealed class BackgroundTask
{
	public event Action? OnTaskChanged;
	public event Action? OnTaskEnded;

	private readonly IBackgroundTaskStepCompletionSink _sink;

	private readonly List<BackgroundTaskStep> _steps = new();
	private readonly List<BackgroundTaskLink> _links = new();
	private readonly Stack<BackgroundTaskStep> _stack = new();

	private string? _statusText;

	private bool _endedNotified;

	public BackgroundTask(BackgroundTaskType type, string name, string description, int expectedSteps, bool isWorkflow, IBackgroundTaskStepCompletionSink sink)
	{
		Type = type;
		Name = name;
		Description = description;
		ExpectedSteps = expectedSteps;
		IsWorkflow = isWorkflow;
		_sink = sink;
	}

	public Guid Id { get; } = Guid.NewGuid();

	public BackgroundTaskType Type { get; }
	public string Name { get; }
	public string Description { get; }
	public bool IsWorkflow { get; }
	public int ExpectedSteps { get; }

	public IReadOnlyList<BackgroundTaskStep> Steps => _steps;
	public IReadOnlyList<BackgroundTaskLink> Links => _links;

	public bool IsOverlayVisible { get; set; } = true;

	public CancellationTokenSource Cts { get; } = new();
	public CancellationToken Ct => Cts.Token;

	public BackgroundTaskStep? CurrentStep => _stack.Count == 0 ? null : _stack.Peek();

	public bool IsCancelRequested { get; private set; }

	public double Progress { get; private set; }

	public DateTimeOffset? StartedAt => Steps.Count == 0 ? null : Steps[0].StartedAt;

	public DateTimeOffset? FinishedAt => Steps.LastOrDefault(s => s.FinishedAt.HasValue)?.FinishedAt;

	public TimeSpan? Duration
	{
		get
		{
			if (!StartedAt.HasValue || !FinishedAt.HasValue)
			{
				return null;
			}

			return FinishedAt.Value - StartedAt.Value;
		}
	}

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

	public string StatusText
	{
		get
		{
			var baseText = Status switch
			{
				BackgroundTaskStatus.Running => "Running",
				BackgroundTaskStatus.Finished => "Finished",
				BackgroundTaskStatus.Failed => "Failed",
				BackgroundTaskStatus.Canceled => "Canceled",
				_ => "Unknown"
			};

			return string.IsNullOrWhiteSpace(_statusText) ? baseText : $"{baseText}: {_statusText}";
		}
	}

	public bool Ended => Status is BackgroundTaskStatus.Finished or BackgroundTaskStatus.Failed or BackgroundTaskStatus.Canceled;

	public bool HasActiveWork => _steps.Any(s => s.Status == BackgroundTaskStatus.Running && !s.IsWaiting && s.Outcome == BackgroundStepOutcome.Executed);

	public bool IsWaitingOnly => _steps.Any(s => s.IsWaiting) && _steps.All(s => s.IsWaiting || s.Outcome == BackgroundStepOutcome.Skipped || s.Status != BackgroundTaskStatus.Running);

	public bool IsNoOp => _steps.Count > 0 && _steps.All(s => s.Outcome == BackgroundStepOutcome.Skipped);

	public void NotifyChange()
	{
		if (IsNoOp)
		{
			return;
		}


		RecalculateProgress();
		OnTaskChanged?.Invoke();
	}

	private void NotifyTaskEnded()
	{
		if (_endedNotified)
		{
			return;
		}

		OnTaskEnded?.Invoke();
		_endedNotified = true;
	}


	public void AddLink(string text, string title, string urlWeb, Enum icon)
	{
		_links.Add(new BackgroundTaskLink(text, title, null, urlWeb, icon));
		NotifyChange();
	}

	public void AddLink(string text, string title, SpotifyIdNameUrlObject spotifyUrlObject, Enum? icon = null)
	{
		icon ??= SpotifyIcon.SmallGreen;
		_links.Add(new BackgroundTaskLink(text, title, spotifyUrlObject.UrlApp, spotifyUrlObject.UrlWeb, icon));
		NotifyChange();
	}

	public Task<bool> WaitForStep(Guid stepId) => _sink.WaitForStep(stepId, Ct);

	public string? GetStepLabel(Guid stepId) => _sink.GetStepLabel(stepId);

	public async Task WaitForStep(BackgroundTaskStep step, Guid dependsOnStepId)
	{
		step.IsWaiting = true;
		step.WaitingForStepId = dependsOnStepId;
		step.NotifyChange();

		try
		{
			var ok = await WaitForStep(dependsOnStepId);

			if (!ok)
			{
				throw new InvalidOperationException($"Dependency step {dependsOnStepId} failed or was canceled.");
			}
		}
		finally
		{
			step.IsWaiting = false;
			step.WaitingForStepId = null;
			step.NotifyChange();
		}
	}

	public void RequestCancel()
	{
		if (IsCancelRequested)
		{
			return;
		}

		IsCancelRequested = true;

		try
		{
			Cts.Cancel();
		}
		catch
		{
			// ignore
		}

		foreach (var s in _steps.Where(s => s.Status == BackgroundTaskStatus.Running))
		{
			s.MarkCanceled();
		}

		NotifyChange();
		NotifyTaskEnded();
	}

	private static string GetDefaultStepName(BackgroundTaskCategory category)
	{
		const string apiName = "Sending API request";
		const string dbGetName = "Getting from DB";
		const string dbSaveName = "Saving to DB";

		var name = category switch
		{
			BackgroundTaskCategory.GetApi => apiName,
			BackgroundTaskCategory.SaveApi => apiName,
			BackgroundTaskCategory.DeleteApi => apiName,
			BackgroundTaskCategory.GetDb => dbGetName,
			BackgroundTaskCategory.SaveDb => dbSaveName,
			BackgroundTaskCategory.DeleteDb => dbSaveName,
			_ => "Working"
		};

		return name;
	}

	public Task RunStep(BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task> work)
	{
		var name = GetDefaultStepName(category);
		return RunStep(name, category, work);
	}

	public Task RunStep(Guid stepId, BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task> work)
	{
		var name = GetDefaultStepName(category);
		return RunStep(stepId, name, category, work);
	}

	public Task RunStep(string name, BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task> work)
	{
		return RunStep(Guid.NewGuid(), name, category, work);
	}

	public Task RunStep(Guid stepId, string name, BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task> work)
	{
		return RunStepInternal(stepId, name, category, async (ct, step) =>
		{
			await work(ct, step);
			return 0;
		});
	}

	public Task<T> RunStep<T>(BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task<T>> work)
	{
		var name = GetDefaultStepName(category);
		return RunStep(name, category, work);
	}

	public Task<T> RunStep<T>(Guid stepId, BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task<T>> work)
	{
		var name = GetDefaultStepName(category);
		return RunStep(stepId, name, category, work);
	}

	public Task<T> RunStep<T>(string name, BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task<T>> work)
	{
		return RunStepInternal(Guid.NewGuid(), name, category, work);
	}

	public Task<T> RunStep<T>(Guid stepId, string name, BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task<T>> work)
	{
		return RunStepInternal(stepId, name, category, work);
	}

	private async Task<T> RunStepInternal<T>(Guid stepId, string name, BackgroundTaskCategory category, Func<CancellationToken, BackgroundTaskStep, Task<T>> work)
	{
		var step = new BackgroundTaskStep { StepId = stepId, Name = name, Category = category, TaskCt = Ct };

		_steps.Add(step);
		_sink.RegisterStep(step.StepId, step.Name);

		step.OnStateChanged += NotifyChange;

		_stack.Push(step);
		NotifyChange();

		try
		{
			var result = await work(Ct, step);

			if (!step.Ended)
			{
				step.MarkFinished();
			}

			_sink.MarkStepCompleted(step.StepId, step.Status == BackgroundTaskStatus.Finished);
			return result;
		}
		catch (OperationCanceledException)
		{
			step.MarkCanceled();
			_sink.MarkStepCompleted(step.StepId, false);
			NotifyTaskEnded();
			throw;
		}
		catch (Exception ex)
		{
			step.MarkFailed(ex);
			_sink.MarkStepCompleted(step.StepId, false);
			_statusText = ex.Message;
			NotifyTaskEnded();
			throw;
		}
		finally
		{
			_stack.Pop();
			step.OnStateChanged -= NotifyChange;
			NotifyChange();
			if (Ended)
			{
				NotifyTaskEnded();
			}
		}
	}

	private void RecalculateProgress()
	{
		if (_steps.Count == 0)
		{
			Progress = 0;
			return;
		}

		var total = ExpectedSteps > 0 ? ExpectedSteps : _steps.Count;
		var done = 0.0;

		foreach (var s in _steps)
		{
			done += Math.Clamp(s.SubProgress, 0, 1) / total;
		}

		Progress = Math.Clamp(done, 0, 1);
	}
}

public sealed class BackgroundTask2(BackgroundTaskType type, string name, string info, int expectedSteps, bool isWorkflow, IBackgroundTaskStepCompletionSink2 stepSink)
{
	public event Action? OnStateChanged;


	public Guid Id { get; } = Guid.NewGuid();

	public string Name { get; init; } = name;

	public string Info { get; init; } = info;

	public bool IsWorkflow { get; init; } = isWorkflow;

	public int ExpectedSteps { get; init; } = expectedSteps;

	public BackgroundTaskType Type { get; init; } = type;

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

	public bool IsCancelRequested { get; internal set; }

	public bool IsEndTaskRequested { get; internal set; }

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


	private readonly List<BackgroundTaskStep2> _steps = [];
	public IReadOnlyList<BackgroundTaskStep2> Steps => _steps;

	private readonly List<BackgroundTaskLink> _links = [];
	public IReadOnlyList<BackgroundTaskLink> Links => _links;

	public CancellationTokenSource Cts { get; } = new();


	public CancellationToken Ct => Cts.Token;

	public BackgroundTaskStep2? CurrentStep => Steps.ElementAtOrDefault(CurrentStepIndex);

	public DateTimeOffset? StartedAt => _steps.FirstOrDefault()?.StartedAt;

	public DateTimeOffset? FinishedAt => _steps.LastOrDefault(s => s.FinishedAt.HasValue)?.FinishedAt;

	public TimeSpan? Duration => FinishedAt.HasValue ? FinishedAt.Value - StartedAt : null;

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

	public IBackgroundTaskStepCompletionSink2 StepSink { get; } = stepSink;

	public void NotifyChange()
	{
		OnStateChanged?.Invoke();
	}

	private void HandleStepChanged()
	{
		RecalculateProgress();
		NotifyChange();
	}

	public void AddStep(BackgroundTaskStep2 step)
	{
		if (IsEndTaskRequested)
		{
			return;
		}

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

		var total = ExpectedSteps > 0 ? ExpectedSteps : Steps.Count;
		var done = 0.0;

		for (int i = 0; i < Steps.Count; i++)
		{
			done += Math.Clamp(Steps[i].SubProgress, 0, 1) / total;
		}

		Progress = Math.Clamp(done, 0, 1);
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

	public Task<bool> WaitForStep(Guid stepId)
	{
		return StepSink.WaitForStep(stepId, Ct);
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

		foreach (var step in Steps.Where(s => s.Status == BackgroundTaskStatus.Running))
		{
			step.MarkCanceled();
		}

		NotifyChange();
	}

	public void MarkFailed(Exception ex)
	{
		_statusText = ex.Message;

		foreach (var step in Steps.Where(s => s.Status == BackgroundTaskStatus.Running))
		{
			step.MarkFailed(ex);
		}

		NotifyChange();
	}
	public void MarkFinished()
	{
		foreach (var step in Steps.Where(s => s.Status == BackgroundTaskStatus.Running))
		{
			step.MarkFinished();
		}
		NotifyChange();
	}
}