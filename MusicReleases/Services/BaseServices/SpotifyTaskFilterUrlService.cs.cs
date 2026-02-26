using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyTaskFilterUrlService : ISpotifyTaskFilterUrlService
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
		var filter = TaskFilter.All;

		if (filterParams.IsNullOrEmpty())
		{
			return filter;
		}

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


	public string ToggleInQuery(string? currentQuery, TaskFilter toToggle, string searchText)
	{
		var filter = ParseFilterFromUrlParams(currentQuery);
		filter ^= toToggle;
		return CreateUrlParams(filter, searchText);
	}


	/*public void ToggleFilter(TaskFilter filter)
	{
		_filter ^= filter;
		_filter = EnsurePairsValid(_filter);
	}*/
}
