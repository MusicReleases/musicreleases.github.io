using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyTaskManagerService : ISpotifyTaskManagerService
{
	public event Action? OnChange;


	public bool IsAnyTaskRunning => RunningTasks.Count > 0;

	public bool IsAnyTaskVisible => VisibleTasks.Count > 0;

	public IReadOnlyList<SpotifyBackgroundTask> AllTasks => _tasks;

	public ICollection<SpotifyBackgroundTask> RunningTasks => [.. _tasks.Where(t => t.IsRunning)];

	public ICollection<SpotifyBackgroundTask> VisibleTasks => [.. _tasks.Where(t => t.IsOverlayVisible)];

	public ICollection<SpotifyBackgroundTask> FilteredTasks => [.. _filterService.Apply(_tasks)];


	private readonly List<SpotifyBackgroundTask> _tasks = [];

	private readonly ISpotifyTaskFilterService _filterService;


	public SpotifyTaskManagerService(ISpotifyTaskFilterService filterService)
	{
		_filterService = filterService;
		_filterService.OnFilterChanged += () => OnChange?.Invoke();
	}

	private void NotifyUI()
	{
		OnChange?.Invoke();
	}

	public async Task Run(string name, Func<SpotifyBackgroundTask, Task> work)
	{
		var task = new SpotifyBackgroundTask
		{
			Name = name,
			Status = "Starting..."
		};

		task.OnStateChanged += NotifyUI;

		_tasks.Insert(0, task);
		NotifyUI();

		try
		{
			await work(task);
			task.Status = "Finished";
			task.Progress = 1.0;
		}
		catch (OperationCanceledException)
		{
			task.Status = "Canceled by user";
			task.Failed = true;
		}
		catch (Exception ex)
		{
			task.Status = $"Error: {ex.Message}";
			task.Failed = true;
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

	private async Task HideAfterDelay(SpotifyBackgroundTask task)
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

	public void RemoveTask(SpotifyBackgroundTask task)
	{
		_tasks.Remove(task);
		NotifyUI();
	}

	public void HideTask(SpotifyBackgroundTask task)
	{
		task.IsOverlayVisible = false;
		NotifyUI();
	}
}
