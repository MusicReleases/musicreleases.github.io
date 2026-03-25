namespace JakubKastner.MusicReleases.Spotify.Tasks;

public sealed record BackgroundTaskRequest(BackgroundTaskType Type, string Name, string Info, int ExpectedSteps, Func<BackgroundTask, Task> Work, IReadOnlyCollection<BackgroundTaskType>? DependsOn = null, bool IsImmediate = false);