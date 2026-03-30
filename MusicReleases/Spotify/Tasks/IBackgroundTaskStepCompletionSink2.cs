namespace JakubKastner.MusicReleases.Spotify.Tasks;

public interface IBackgroundTaskStepCompletionSink
{
	event Action? OnUiRelevantChange;

	void RegisterStep(Guid stepId, string label);

	void MarkStepCompleted(Guid stepId, bool success);
	Task<bool> WaitForStep(Guid stepId, CancellationToken ct);
	string? GetStepLabel(Guid stepId);

}

public interface IBackgroundTaskStepCompletionSink2
{
	void MarkStepCompleted(Guid stepId, bool success);
	Task<bool> WaitForStep(Guid stepId, CancellationToken ct);
}
