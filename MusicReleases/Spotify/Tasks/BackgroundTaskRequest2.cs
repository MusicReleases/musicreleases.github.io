namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed record BackgroundTaskRequest
(
	BackgroundTaskType Type,
	string Name,
	string Description,
	int ExpectedSteps,
	Func<BackgroundTask, Task> Work,
	IReadOnlyCollection<BackgroundTaskType>? DependsOn = null,
	bool IsImmediate = false
)
{
	public Guid RequestId { get; init; } = Guid.NewGuid();
};

internal sealed record BackgroundTaskRequest2
(
	BackgroundTaskType Type,
	string Name,
	string Info,
	int ExpectedSteps,
	Func<BackgroundTask2, Task> Work,
	IReadOnlyCollection<BackgroundTaskType>? DependsOn = null,
	bool IsImmediate = false
)
{
	public Guid RequestId { get; init; } = Guid.NewGuid();
};