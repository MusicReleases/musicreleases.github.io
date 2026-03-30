namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class BackgroundTaskFilter
{
	public BackgroundTaskFilterType TaskFilter { get; set; } = BackgroundTaskFilterType.All;

	public BackgroundTaskFilter()
	{ }

	public BackgroundTaskFilter(BackgroundTaskFilterType taskFilter)
	{
		TaskFilter = taskFilter;
	}
}
