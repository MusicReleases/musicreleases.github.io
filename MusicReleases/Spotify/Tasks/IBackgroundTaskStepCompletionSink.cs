namespace JakubKastner.MusicReleases.Spotify.Tasks;

public interface IBackgroundTaskStepCompletionSink
{
	void MarkStepCompleted(Guid stepId, bool success);
	Task<bool> WaitForStep(Guid stepId, CancellationToken ct);
}
