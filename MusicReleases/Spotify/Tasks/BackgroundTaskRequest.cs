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