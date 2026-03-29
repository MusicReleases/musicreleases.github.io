namespace JakubKastner.MusicReleases.Spotify.Tasks;

//internal readonly record struct BackgroundTaskSyncPlan(BackgroundTaskSyncStepIds Steps, Guid? WaitBeforeDbLoad = null, Guid? WaitBeforeApiLoad = null);


public sealed record BackgroundTaskSyncPlan
(
	Guid DbStepId,
	Guid ApiStepId,
	Guid SaveStepId,
	Guid? WaitBeforeDbStepId = null,
	Guid? WaitBeforeApiStepId = null
);
