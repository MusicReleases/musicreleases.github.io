using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class SpotifyTaskFilterUrlService : IBackgroundTaskFilterUrlService
{
	public string CreateUrlParams(TaskFilter filter, string? searchText)
	{
		var urlParams = new List<string>();

		if (filter != TaskFilter.All)
		{
			var flags = Enum.GetValues<TaskFilter>().Where(f => f != TaskFilter.All && filter.HasFlag(f));

			urlParams.Add("filter=" + string.Join(",", flags));
		}

		if (searchText.IsNotNullOrEmpty())
		{
			urlParams.Add("search=" + searchText);
		}

		if (urlParams.Count == 0)
		{
			return string.Empty;
		}

		return $"?{string.Join("&", urlParams)}";
	}

	public TaskFilter ParseFilterFromUrlParams(string? filterParams)
	{

		if (filterParams.IsNullOrEmpty())
		{
			return TaskFilter.All;
		}

		var filter = (TaskFilter)0;

		var parts = filterParams.Split(',', StringSplitOptions.RemoveEmptyEntries);

		foreach (var p in parts)
		{
			if (Enum.TryParse<TaskFilter>(p, true, out var parsed))
			{
				filter |= parsed;
			}
		}
		return filter;
	}
}
