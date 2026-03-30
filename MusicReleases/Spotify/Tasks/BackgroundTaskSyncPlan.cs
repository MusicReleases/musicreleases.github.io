namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed record BackgroundTaskSyncPlan
(
	Guid DbStepId,
	Guid ApiStepId,
	Guid SaveStepId,
	Guid? WaitBeforeDbStepId = null,
	Guid? WaitBeforeApiStepId = null
);
