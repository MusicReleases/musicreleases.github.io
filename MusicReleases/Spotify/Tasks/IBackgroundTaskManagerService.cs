namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskManagerService
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
	void Dispose();
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