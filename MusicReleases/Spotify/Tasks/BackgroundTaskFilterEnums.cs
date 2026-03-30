namespace JakubKastner.MusicReleases.Spotify.Tasks;

[Flags]
public enum BackgroundTaskFilterType
{
	Running = 1 << 0, // 1
	Canceled = 1 << 1, // 2
	Failed = 1 << 2, // 4
	Finished = 1 << 3, // 8

	All = Running | Canceled | Failed | Finished,
}