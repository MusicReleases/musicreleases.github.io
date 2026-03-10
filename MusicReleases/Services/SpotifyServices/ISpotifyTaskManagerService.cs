using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Services.SpotifyServices
{
	public interface ISpotifyTaskManagerService
	{
		IReadOnlyList<BackgroundTask> AllTasks { get; }
		ICollection<BackgroundTask> FilteredTasks { get; }
		bool IsAnyTaskRunning { get; }
		bool IsAnyTaskVisible { get; }
		ICollection<BackgroundTask> RunningTasks { get; }
		ICollection<BackgroundTask> VisibleTasks { get; }

		event Action? OnChange;

		void Dispose();
		void HideAllFinished();
		void HideTask(BackgroundTask task);
		void RemoveTask(BackgroundTask task);
		Task Run(string name, BackgroundTaskType type, Func<BackgroundTask, Task> work);
	}
}