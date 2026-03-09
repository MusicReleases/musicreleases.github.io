using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public interface ISpotifyTaskManagerService
{
	IReadOnlyList<SpotifyBackgroundTask> AllTasks { get; }
	ICollection<SpotifyBackgroundTask> FilteredTasks { get; }
	bool IsAnyTaskRunning { get; }
	ICollection<SpotifyBackgroundTask> RunningTasks { get; }
	ICollection<SpotifyBackgroundTask> VisibleTasks { get; }
	bool IsAnyTaskVisible { get; }

	event Action? OnChange;

	void HideAllFinished();
	void HideTask(SpotifyBackgroundTask task);
	void RemoveTask(SpotifyBackgroundTask task);
	Task Run(string name, Func<SpotifyBackgroundTask, Task> work);
}