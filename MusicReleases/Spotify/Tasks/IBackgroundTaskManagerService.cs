namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskManagerService
{
	IReadOnlyList<BackgroundTask> AllTasks { get; }
	bool AnyTaskFailed { get; }
	IReadOnlyList<BackgroundTask> VisibleTasks { get; }

	event Action? OnChange;

	void CancelAllTasks();
	void Dispose();
	Task Enqueue(BackgroundTaskRequest request);
	void HideAllEnded();
	void HideTask(BackgroundTask task);
	bool IsEffectivelyLoading(BackgroundTaskType type);
	Task Run(BackgroundTaskType type, string name, string description, int expectedSteps, Func<BackgroundTask, Task> work);
	Task Run(BackgroundTaskType type, string name, string description, Func<BackgroundTask, Task> work);
	void StartWorkflow();
}