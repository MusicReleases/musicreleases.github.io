using JakubKastner.MusicReleases.Objects.Spotify;
using System.Collections.ObjectModel;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyTaskManagerService : ISpotifyTaskManagerService
{
	public event Action? OnChange;
	public ObservableCollection<SpotifyBackgroundTask> Tasks { get; } = [];
	public bool IsAnyTaskRunning => Tasks.Any(t => t.IsRunning);

	private void NotifyUI()
	{
		OnChange?.Invoke();
	}

	public async Task Run(string name, Func<SpotifyBackgroundTask, Task> work)
	{
		var task = new SpotifyBackgroundTask { Name = name, Status = "Starting..." };
		Tasks.Insert(0, task);
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
			NotifyUI();
		}
	}
}
