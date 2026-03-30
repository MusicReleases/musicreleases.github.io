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

	public event Action? OnChange;

	public IReadOnlyList<BackgroundTask> AllTasks => _tasks;

	public IReadOnlyList<BackgroundTask> VisibleTasks => _tasks.Where(t => t.IsOverlayVisible && t.Steps.Any(x => x.Outcome != BackgroundStepOutcome.Skipped)).ToList();

	public bool AnyTaskFailed => _tasks.Any(t => t.Status == BackgroundTaskStatus.Failed);

	public bool AnyTaskVisible => _tasks.Any(t => t.IsOverlayVisible);

	public void Dispose()
	{
		GC.SuppressFinalize(this);
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

		NotifyUI();
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
		NotifyUI();
	}

	public void HideAllEnded()
	{
		foreach (var t in _tasks.Where(t => t.Ended))
		{
			t.IsOverlayVisible = false;
		}

		NotifyUI();
	}

	public void RemoveCompletedTask(BackgroundTask task)
	{
		if (!(task.Status is BackgroundTaskStatus.Finished or BackgroundTaskStatus.Failed or BackgroundTaskStatus.Canceled))
		{
			return;
		}

		_tasks.Remove(task);
		NotifyUI();
	}

	public void RemoveAllCompleted()
	{
		_tasks.RemoveAll(t => t.Status is BackgroundTaskStatus.Finished or BackgroundTaskStatus.Failed or BackgroundTaskStatus.Canceled);

		NotifyUI();
	}


	public void CancelAllTasks()
	{
		foreach (var t in _tasks.Where(t => !t.Ended))
		{
			t.RequestCancel();
		}

		NotifyUI();
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

	private void NotifyUI()
	{
		OnChange?.Invoke();
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
		task.OnStateChanged += NotifyUI;

		_tasks.Insert(0, task);
		NotifyUI();

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
			task.OnStateChanged -= NotifyUI;
			NotifyUI();

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



internal sealed class BackgroundTaskManagerService2 : IBackgroundTaskManagerService2, IBackgroundTaskStepCompletionSink2
{
	private readonly IBackgroundTaskFilterService _filterService;

	private readonly List<BackgroundTask2> _tasks = [];


	private readonly List<BackgroundTaskRequest2> _workflowQueue = [];
	private readonly HashSet<BackgroundTaskType> _completedWorkflowTasks = [];
	private bool _workflowRunning;


	private readonly object _workflowLock = new();
	private readonly HashSet<Guid> _startedWorkflowRequests = new();

	private readonly ConcurrentDictionary<Guid, TaskCompletionSource<bool>> _stepCompletions = new();

	public BackgroundTaskManagerService2(IBackgroundTaskFilterService filterService)
	{
		_filterService = filterService;
		_filterService.OnFilterChanged += NotifyUI;
	}

	public void Dispose()
	{
		_filterService.OnFilterChanged -= NotifyUI;
		GC.SuppressFinalize(this);
	}


	public event Action? OnChange;

	public bool IsAnyTaskRunning => RunningTasks.Count > 0;

	public bool IsAnyTaskVisible => VisibleTasks.Count > 0;

	public bool AnyTaskFailed => _tasks.Any(t => t.Failed);

	public IReadOnlyList<BackgroundTask2> AllTasks => _tasks;

	public ICollection<BackgroundTask2> RunningTasks => [.. _tasks.Where(t => t.IsRunning)];

	public ICollection<BackgroundTask2> VisibleTasks => [.. _tasks.Where(t => t.IsOverlayVisible /*&& !t.IsWorkflow*/)];

	//public ICollection<BackgroundTask2> FilteredTasks => [.. _filterService.Apply(_tasks)];


	private void NotifyUI()
	{
		OnChange?.Invoke();
	}

	public void StartWorkflow()
	{
		_completedWorkflowTasks.Clear();
		_workflowQueue.Clear();


		lock (_workflowLock)
		{
			_startedWorkflowRequests.Clear();
		}
	}


	public Task Enqueue(BackgroundTaskRequest2 request)
	{
		if (request.IsImmediate)
		{
			return RunInternal(request.Type, request.Name, request.Info, request.ExpectedSteps, request.Work, true);
		}

		// dedup (task is allready running or queued)
		if (_workflowQueue.Any(t => t.Type == request.Type))
		{
			return Task.CompletedTask;
		}

		// task queue
		_workflowQueue.Add(request);
		StartReadyWorkflowTasks();

		return Task.CompletedTask;
	}


	private void StartReadyWorkflowTasks()
	{
		List<BackgroundTaskRequest2> toStart = [];

		lock (_workflowLock)
		{
			foreach (var req in _workflowQueue)
			{
				// is running?
				if (_startedWorkflowRequests.Contains(req.RequestId))
				{
					continue;
				}

				// dependencies finished?
				if (req.DependsOn is not null && !req.DependsOn.All(d => _completedWorkflowTasks.Contains(d)))
				{
					continue;
				}

				_startedWorkflowRequests.Add(req.RequestId);
				toStart.Add(req);
			}
		}

		// start outside lock
		foreach (var req in toStart)
		{
			_ = RunWorkflowRequestAsync(req);
		}
	}

	private async Task RunWorkflowRequestAsync(BackgroundTaskRequest2 req)
	{
		try
		{
			await RunInternal(req.Type, req.Name, req.Info, req.ExpectedSteps, req.Work, true);

			lock (_workflowLock)
			{
				_completedWorkflowTasks.Add(req.Type);
				_workflowQueue.Remove(req);
			}
		}
		finally
		{
			// unlock other tasks
			StartReadyWorkflowTasks();
		}
	}

	public Task Run(BackgroundTaskType type, string name, string info, Func<BackgroundTask2, Task> work)
	{
		var expectedSteps = type switch
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

		return RunInternal(type, name, info, expectedSteps, work);
	}

	public Task Run(BackgroundTaskType type, string name, string info, int expectedSteps, Func<BackgroundTask2, Task> work)
	{
		return RunInternal(type, name, info, expectedSteps, work);
	}

	private async Task TryRunNextWorkflow()
	{
		if (_workflowRunning)
		{
			return;
		}

		var next = _workflowQueue.FirstOrDefault(req => req.DependsOn is null || req.DependsOn.All(d => _completedWorkflowTasks.Contains(d)));

		if (next is null)
		{
			return;
		}

		_workflowRunning = true;

		try
		{
			await RunInternal(next.Type, next.Name, next.Info, next.ExpectedSteps, next.Work, true);

			// finished task
			_completedWorkflowTasks.Add(next.Type);
			_workflowQueue.Remove(next);
		}
		finally
		{
			_workflowRunning = false;
			await TryRunNextWorkflow();
		}
	}

	private Task RunInternal(BackgroundTaskType type, string name, string info, int expectedSteps, Func<BackgroundTask2, Task> work, bool isWorkflow = false)
	{
		var task = new BackgroundTask2(type, name, info, expectedSteps, isWorkflow, this);
		task.OnStateChanged += NotifyUI;

		_tasks.Insert(0, task);
		NotifyUI();

		return RunCoreAsync(task, work);
	}

	private async Task RunCoreAsync(BackgroundTask2 task, Func<BackgroundTask2, Task> work)
	{
		try
		{
			await work(task);

			if (task.IsEndTaskRequested || (!task.IsCancelRequested && !task.Ended))
			{
				task.MarkFinished();
			}

			task.RecalculateProgress();
		}
		catch (OperationCanceledException)
		{
			if (task.IsEndTaskRequested)
			{
				task.MarkFinished();
				return;
			}
			task.MarkCanceled();
		}
		catch (Exception ex)
		{
			task.MarkFailed(ex);
		}
		finally
		{
			task.RecalculateProgress();
			task.OnStateChanged -= NotifyUI;
			NotifyUI();
			_ = HideAfterDelay(task);
		}
	}

	private async Task HideAfterDelay(BackgroundTask2 task)
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

	void IBackgroundTaskStepCompletionSink2.MarkStepCompleted(Guid stepId, bool success)
	{
		var tcs = _stepCompletions.GetOrAdd(stepId, _ => new(TaskCreationOptions.RunContinuationsAsynchronously));

		if (success)
		{
			tcs.TrySetResult(true);
		}
		else
		{
			tcs.TrySetResult(false);
		}
	}

	Task<bool> IBackgroundTaskStepCompletionSink2.WaitForStep(Guid stepId, CancellationToken ct)
	{
		var tcs = _stepCompletions.GetOrAdd(stepId, _ => new(TaskCreationOptions.RunContinuationsAsynchronously));

		if (ct.CanBeCanceled)
		{
			ct.Register(() => tcs.TrySetCanceled(ct));
		}

		return tcs.Task;
	}



	public void HideAllEnded()
	{
		foreach (var task in _tasks.Where(t => t.Ended))
		{
			task.IsOverlayVisible = false;
		}
		NotifyUI();
	}

	public void RemoveTask(BackgroundTask2 task)
	{
		if (task.IsRunning)
		{
			return;
		}

		_tasks.Remove(task);
		NotifyUI();
	}

	public void RemoveAllFinishedTasks()
	{
		_tasks.RemoveAll(t => t.Ended);
		NotifyUI();
	}

	public void HideTask(BackgroundTask2 task)
	{
		task.IsOverlayVisible = false;
		NotifyUI();
	}

	public void CancelAllTasks()
	{
		foreach (var task in RunningTasks)
		{
			if (task.Ended)
			{
				return;
			}
			task.RequestCancel();
		}
		NotifyUI();
	}
}