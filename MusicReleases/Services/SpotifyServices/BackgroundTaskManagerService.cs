using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class BackgroundTaskManagerService : IDisposable, IBackgroundTaskManagerService
{
	private readonly IBackgroundTaskFilterService _filterService;

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

	public IReadOnlyList<BackgroundTask> AllTasks => _tasks;

	public ICollection<BackgroundTask> RunningTasks => [.. _tasks.Where(t => t.IsRunning)];

	public ICollection<BackgroundTask> VisibleTasks => [.. _tasks.Where(t => t.IsOverlayVisible)];

	public ICollection<BackgroundTask> FilteredTasks => [.. _filterService.Apply(_tasks)];


	private readonly List<BackgroundTask> _tasks = [];


	private void NotifyUI()
	{
		OnChange?.Invoke();
	}


	public Task Run(BackgroundTaskType type, string name, string info, Func<BackgroundTask, Task> work)
	{
		var task = new BackgroundTask(type, name, info);
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

			task.MarkFinished();
			task.RecalculateProgress();
		}
		catch (OperationCanceledException)
		{
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
}