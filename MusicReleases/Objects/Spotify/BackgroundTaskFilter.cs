using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Objects.Spotify;

public class BackgroundTaskFilter
{
	public TaskFilter TaskFilter { get; set; } = TaskFilter.All;

	public BackgroundTaskFilter()
	{

	}

	public BackgroundTaskFilter(TaskFilter taskFilter)
	{
		TaskFilter = taskFilter;
	}
}
