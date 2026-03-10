using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class SpotifyTaskManagerService : IDisposable, ISpotifyTaskManagerService
{
	private readonly ISpotifyTaskFilterService _filterService;

	public SpotifyTaskManagerService(ISpotifyTaskFilterService filterService)
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

	public async Task Run(string name, BackgroundTaskType type, Func<BackgroundTask, Task> work)
	{
		var task = new BackgroundTask(name, type);

		task.OnStateChanged += NotifyUI;

		_tasks.Insert(0, task);
		NotifyUI();

		try
		{
			await work(task);
			task.Status = "Finished";
			task.Progress = 1.0;


			var current = task.Steps.ElementAtOrDefault(task.CurrentStepIndex);
			if (current is not null && current.Status == BackgroundTaskStatus.Running)
			{
				current.MarkCanceled();
			}

		}
		catch (OperationCanceledException)
		{
			task.Status = "Canceled by user";
			task.Failed = true;


			var current = task.Steps.ElementAtOrDefault(task.CurrentStepIndex);
			if (current is not null && current.Status == BackgroundTaskStatus.Running)
			{
				current.MarkCanceled();
			}
		}
		catch (Exception ex)
		{
			task.Status = $"Error: {ex.Message}";
			task.Failed = true;


			var current = task.Steps.ElementAtOrDefault(task.CurrentStepIndex);
			if (current is not null && current.Status == BackgroundTaskStatus.Running)
			{
				current.MarkFailed(ex);
			}


			Console.WriteLine(ex);
		}
		finally
		{
			task.IsRunning = false;
			task.OnStateChanged -= NotifyUI;
			NotifyUI();

			_ = HideAfterDelay(task);
		}
	}

	private async Task HideAfterDelay(BackgroundTask task)
	{
		await Task.Delay(task.Failed ? 10000 : 5000);

		task.IsOverlayVisible = false;
		NotifyUI();
	}

	public void HideAllFinished()
	{
		foreach (var task in _tasks.Where(t => !t.IsRunning))
		{
			task.IsOverlayVisible = false;
		}
		NotifyUI();
	}

	public void RemoveTask(BackgroundTask task)
	{
		_tasks.Remove(task);
		NotifyUI();
	}

	public void HideTask(BackgroundTask task)
	{
		task.IsOverlayVisible = false;
		NotifyUI();
	}
}
