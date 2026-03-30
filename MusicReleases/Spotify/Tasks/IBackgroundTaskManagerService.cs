namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskManagerService : IDisposable
{
	IReadOnlyList<BackgroundTask> AllTasks { get; }
	bool AnyTaskFailed { get; }
	IReadOnlyList<BackgroundTask> VisibleTasks { get; }
	bool AnyTaskVisible { get; }
	bool AnyTaskRunning { get; }
	IReadOnlyList<BackgroundTask> RunningTasks { get; }
	IReadOnlyList<BackgroundTask> NotSkippedTasks { get; }

	event Action? OnUiRelevantChange;

	void CancelAllTasks();
	Task Enqueue(BackgroundTaskRequest request);
	void HideAllEnded();
	void HideTask(BackgroundTask task);
	bool IsEffectivelyLoading(BackgroundTaskType type);
	void RemoveAllCompleted();
	void RemoveCompletedTask(BackgroundTask task);
	Task Run(BackgroundTaskType type, string name, string description, int expectedSteps, Func<BackgroundTask, Task> work);
	Task Run(BackgroundTaskType type, string name, string description, Func<BackgroundTask, Task> work);
	void StartWorkflow();
}