namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskManagerService2 : IDisposable
{
	IReadOnlyList<BackgroundTask2> AllTasks { get; }
	//ICollection<BackgroundTask2> FilteredTasks { get; }
	bool IsAnyTaskRunning { get; }
	bool IsAnyTaskVisible { get; }
	ICollection<BackgroundTask2> RunningTasks { get; }
	ICollection<BackgroundTask2> VisibleTasks { get; }
	bool AnyTaskFailed { get; }

	event Action? OnChange;

	void CancelAllTasks();
	Task Enqueue(BackgroundTaskRequest2 request);
	void HideAllEnded();
	void HideTask(BackgroundTask2 task);
	void RemoveAllFinishedTasks();
	void RemoveTask(BackgroundTask2 task);
	Task Run(BackgroundTaskType type, string name, string info, Func<BackgroundTask2, Task> work);
	Task Run(BackgroundTaskType type, string name, string info, int expectedSteps, Func<BackgroundTask2, Task> work);
	void StartWorkflow();
}