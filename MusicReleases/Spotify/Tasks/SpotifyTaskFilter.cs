namespace JakubKastner.MusicReleases.Spotify.Tasks;

public class SpotifyTaskFilter
{
	public TaskFilter TaskFilter { get; set; } = TaskFilter.All;

	public SpotifyTaskFilter()
	{ }

	public SpotifyTaskFilter(TaskFilter taskFilter)
	{
		TaskFilter = taskFilter;
	}
}
