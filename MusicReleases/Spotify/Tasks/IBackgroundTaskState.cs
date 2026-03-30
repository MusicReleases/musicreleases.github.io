namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskState
{
	IReadOnlyList<BackgroundTask> Tasks { get; }

	event Action? OnChange;

	void Dispose();
}