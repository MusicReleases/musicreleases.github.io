using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class BackgroundTaskManagerService : IBackgroundTaskManagerService, IBackgroundTaskStepCompletionSink, IDisposable
{
	private readonly List<BackgroundTask> _tasks = [];

	private readonly List<BackgroundTaskRequest> _workflowQueue = [];
	private readonly HashSet<Guid> _startedWorkflowRequests = [];
	private readonly HashSet<BackgroundTaskType> _completedWorkflowTasks = [];

	private readonly Lock _workflowLock = new();

	private readonly ConcurrentDictionary<Guid, TaskCompletionSource<bool>> _stepCompletions = new();
	private readonly ConcurrentDictionary<Guid, string> _stepLabels = new();

	public event Action? OnUiRelevantChange;


	public IReadOnlyList<BackgroundTask> AllTasks => _tasks;

	public IReadOnlyList<BackgroundTask> VisibleTasks => _tasks.Where(t => t.IsOverlayVisible && t.Steps.Any(x => x.Outcome != BackgroundStepOutcome.Skipped)).ToList();

	public IReadOnlyList<BackgroundTask> RunningTasks => _tasks.Where(t => t.Status == BackgroundTaskStatus.Running).ToList();

	public bool AnyTaskFailed => _tasks.Any(t => t.Status == BackgroundTaskStatus.Failed);

	public bool AnyTaskRunning => _tasks.Any(t => t.Status == BackgroundTaskStatus.Running);

	public bool AnyTaskVisible => _tasks.Any(t => t.IsOverlayVisible);

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	private void NotifyUiRelevant()
	{
		OnUiRelevantChange?.Invoke();
	}

	public bool IsEffectivelyLoading(BackgroundTaskType type)
	{
		return _tasks.Any(t => t.Type == type && t.HasActiveWork);
	}

	public void StartWorkflow()
	{
		lock (_workflowLock)
		{
			_workflowQueue.Clear();
			_startedWorkflowRequests.Clear();
			_completedWorkflowTasks.Clear();
		}

		NotifyUiRelevant();
	}

	public Task Enqueue(BackgroundTaskRequest request)
	{
		if (request.IsImmediate)
		{
			return RunInternal(request.Type, request.Name, request.Description, request.ExpectedSteps, request.Work, isWorkflow: true);
		}

		lock (_workflowLock)
		{
			if (_workflowQueue.Any(t => t.Type == request.Type))
			{
				return Task.CompletedTask;
			}

			_workflowQueue.Add(request);
		}

		StartReadyWorkflowTasks();
		return Task.CompletedTask;
	}

	public Task Run(BackgroundTaskType type, string name, string description, Func<BackgroundTask, Task> work)
	{
		var expected = GuessExpectedSteps(type);
		return Run(type, name, description, expected, work);
	}

	public Task Run(BackgroundTaskType type, string name, string description, int expectedSteps, Func<BackgroundTask, Task> work)
	{
		return RunInternal(type, name, description, expectedSteps, work, isWorkflow: false);
	}

	public void HideTask(BackgroundTask task)
	{
		task.IsOverlayVisible = false;
		NotifyUiRelevant();
	}

	public void HideAllEnded()
	{
		foreach (var t in _tasks.Where(t => t.Ended))
		{
			t.IsOverlayVisible = false;
		}

		NotifyUiRelevant();
	}

	public void RemoveCompletedTask(BackgroundTask task)
	{
		if (!(task.Status is BackgroundTaskStatus.Finished or BackgroundTaskStatus.Failed or BackgroundTaskStatus.Canceled))
		{
			return;
		}

		_tasks.Remove(task);
		NotifyUiRelevant();
	}

	public void RemoveAllCompleted()
	{
		_tasks.RemoveAll(t => t.Status is BackgroundTaskStatus.Finished or BackgroundTaskStatus.Failed or BackgroundTaskStatus.Canceled);

		NotifyUiRelevant();
	}


	public void CancelAllTasks()
	{
		foreach (var t in _tasks.Where(t => !t.Ended))
		{
			t.RequestCancel();
		}

		//NotifyUiRelevant();
	}

	void IBackgroundTaskStepCompletionSink.RegisterStep(Guid stepId, string label)
	{
		_stepLabels[stepId] = label;
	}

	string? IBackgroundTaskStepCompletionSink.GetStepLabel(Guid stepId)
	{
		return _stepLabels.TryGetValue(stepId, out var label) ? label : null;
	}

	void IBackgroundTaskStepCompletionSink.MarkStepCompleted(Guid stepId, bool success)
	{
		var tcs = _stepCompletions.GetOrAdd(stepId, _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));
		tcs.TrySetResult(success);
	}

	Task<bool> IBackgroundTaskStepCompletionSink.WaitForStep(Guid stepId, CancellationToken ct)
	{
		var tcs = _stepCompletions.GetOrAdd(stepId, _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));

		if (ct.CanBeCanceled)
		{
			ct.Register(() => tcs.TrySetCanceled(ct));
		}

		return tcs.Task;
	}

	private void StartReadyWorkflowTasks()
	{
		List<BackgroundTaskRequest> toStart = new();

		lock (_workflowLock)
		{
			foreach (var req in _workflowQueue)
			{
				if (_startedWorkflowRequests.Contains(req.RequestId))
				{
					continue;
				}

				if (req.DependsOn is not null && !req.DependsOn.All(d => _completedWorkflowTasks.Contains(d)))
				{
					continue;
				}

				_startedWorkflowRequests.Add(req.RequestId);
				toStart.Add(req);
			}
		}

		foreach (var req in toStart)
		{
			_ = RunWorkflowRequestAsync(req);
		}
	}

	private async Task RunWorkflowRequestAsync(BackgroundTaskRequest req)
	{
		try
		{
			await RunInternal(req.Type, req.Name, req.Description, req.ExpectedSteps, req.Work, isWorkflow: true);

			lock (_workflowLock)
			{
				_completedWorkflowTasks.Add(req.Type);
				_workflowQueue.Remove(req);
			}
		}
		finally
		{
			StartReadyWorkflowTasks();
		}
	}

	private Task RunInternal(BackgroundTaskType type, string name, string description, int expectedSteps, Func<BackgroundTask, Task> work, bool isWorkflow)
	{
		var task = new BackgroundTask(type, name, description, expectedSteps, isWorkflow, this);
		task.OnTaskEnded += NotifyUiRelevant;

		_tasks.Insert(0, task);
		NotifyUiRelevant();

		return RunCoreAsync(task, work);
	}

	private async Task RunCoreAsync(BackgroundTask task, Func<BackgroundTask, Task> work)
	{
		try
		{
			await work(task);
			task.NotifyChange();
		}
		catch (OperationCanceledException)
		{
			task.RequestCancel();
		}
		catch
		{
			task.NotifyChange();
			throw;
		}
		finally
		{
			task.OnTaskEnded -= NotifyUiRelevant;
			NotifyUiRelevant();

			_ = HideAfterDelay(task);
		}
	}

	private async Task HideAfterDelay(BackgroundTask task)
	{
		var delay = task.Status switch
		{
			BackgroundTaskStatus.Failed => 10000,
			BackgroundTaskStatus.Canceled => 7000,
			_ => 5000
		};

		await Task.Delay(delay);
		HideTask(task);
	}

	private static int GuessExpectedSteps(BackgroundTaskType type)
	{
		return type switch
		{
			BackgroundTaskType.ArtistsGet => 3,

			BackgroundTaskType.ReleasesGet => 3,
			BackgroundTaskType.ReleaseTracksGet => 1,

			BackgroundTaskType.PlaylistsGet => 3,
			BackgroundTaskType.PlaylistsCreate => 2,

			BackgroundTaskType.PlaylistTracksGet => 3,
			BackgroundTaskType.PlaylistTracksAdd => 2,
			BackgroundTaskType.PlaylistTracksRemove => 2,

			_ => 1
		};
	}
}