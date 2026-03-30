namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskState : IDisposable
{
	IReadOnlyList<BackgroundTask> Tasks { get; }

	event Action? OnChange;
}