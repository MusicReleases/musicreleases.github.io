using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.BackgroundTasks.Services;

internal interface IBackgroundTaskManagerService : IDisposable
{
	IReadOnlyList<BackgroundTask> AllTasks { get; }
	ICollection<BackgroundTask> FilteredTasks { get; }
	bool IsAnyTaskRunning { get; }
	bool IsAnyTaskVisible { get; }
	ICollection<BackgroundTask> RunningTasks { get; }
	ICollection<BackgroundTask> VisibleTasks { get; }
	bool AnyTaskFailed { get; }

	event Action? OnChange;

	void HideAllEnded();
	void HideTask(BackgroundTask task);
	void RemoveAllFinishedTasks();
	void RemoveTask(BackgroundTask task);
	Task Run(BackgroundTaskType type, string name, string info, Func<BackgroundTask, Task> work);
	Task Run(BackgroundTaskType type, string name, string info, int expectedSteps, Func<BackgroundTask, Task> work);
}