namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class BackgroundTaskManagerService : IBackgroundTaskManagerService
{
	private readonly IBackgroundTaskFilterService _filterService;

	private readonly List<BackgroundTask> _tasks = [];


	private readonly List<BackgroundTaskRequest> _workflowQueue = [];
	private readonly HashSet<BackgroundTaskType> _completedWorkflowTasks = [];
	private bool _workflowRunning;


	public BackgroundTaskManagerService(IBackgroundTaskFilterService filterService)
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

	public IReadOnlyList<BackgroundTask> AllTasks => _tasks;

	public ICollection<BackgroundTask> RunningTasks => [.. _tasks.Where(t => t.IsRunning)];

	public ICollection<BackgroundTask> VisibleTasks => [.. _tasks.Where(t => t.IsOverlayVisible && !t.IsWorkflow)];

	public ICollection<BackgroundTask> FilteredTasks => [.. _filterService.Apply(_tasks)];


	private void NotifyUI()
	{
		OnChange?.Invoke();
	}

	public void StartWorkflow()
	{
		_completedWorkflowTasks.Clear();
		_workflowQueue.Clear();
	}

	public Task Enqueue(BackgroundTaskRequest request)
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
		_ = TryRunNextWorkflow();

		return Task.CompletedTask;
	}

	public Task Run(BackgroundTaskType type, string name, string info, Func<BackgroundTask, Task> work)
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

	public Task Run(BackgroundTaskType type, string name, string info, int expectedSteps, Func<BackgroundTask, Task> work)
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

	private Task RunInternal(BackgroundTaskType type, string name, string info, int expectedSteps, Func<BackgroundTask, Task> work, bool isWorkflow = false)
	{
		var task = new BackgroundTask(type, name, info, expectedSteps, isWorkflow);
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

	public void HideAllEnded()
	{
		foreach (var task in _tasks.Where(t => t.Ended))
		{
			task.IsOverlayVisible = false;
		}
		NotifyUI();
	}

	public void RemoveTask(BackgroundTask task)
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

	public void HideTask(BackgroundTask task)
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