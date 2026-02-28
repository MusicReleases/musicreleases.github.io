using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyTaskFilterService : ISpotifyTaskFilterService
{
	public event Action? OnFilterChanged;

	public string? SearchText { get; private set; } = null;

	public TaskFilter Filter { get; private set; } = _defaultFilter;

	public bool IsFilterActive => Filter != _defaultFilter;


	private const TaskFilter _defaultFilter = TaskFilter.All;


	private static readonly (TaskFilter A, TaskFilter B)[] FilterPairs =
	[
		(TaskFilter.Visible,   TaskFilter.Hidden),
		(TaskFilter.Running,   TaskFilter.Finished),
		(TaskFilter.Failed,    TaskFilter.Succeeded)
	];

	private static readonly Dictionary<TaskFilter, Func<SpotifyBackgroundTask, bool>> FilterPredicates =
	new()
	{
		[TaskFilter.Visible] = t => t.IsOverlayVisible,
		[TaskFilter.Hidden] = t => !t.IsOverlayVisible,

		[TaskFilter.Running] = t => t.IsRunning,
		[TaskFilter.Finished] = t => !t.IsRunning,

		[TaskFilter.Failed] = t => t.Failed,
		[TaskFilter.Succeeded] = t => !t.Failed,
	};


	private void SetFilterAndSearchInternal(TaskFilter newFilter, string? newSearchText)
	{
		if (newFilter == Filter && string.Equals(newSearchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		Filter = newFilter;
		SearchText = newSearchText;

		OnFilterChanged?.Invoke();
	}

	private void SetFilterInternal(TaskFilter newFilter)
	{
		if (newFilter == Filter)
		{
			return;
		}
		Filter = newFilter;

		OnFilterChanged?.Invoke();
	}

	private void SetSearchInternal(string? newSearchText)
	{
		if (string.Equals(newSearchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		SearchText = newSearchText;

		OnFilterChanged?.Invoke();
	}

	public void SetFilterAndSearch(TaskFilter filter, string? searchText)
	{
		var newFilter = EnsureFilterPairs(filter);
		var newSearchText = EnsudeSearchText(searchText);

		SetFilterAndSearchInternal(newFilter, newSearchText);
	}

	public void SetSearch(string? searchText)
	{
		var newSearchText = EnsudeSearchText(searchText);
		SetSearchInternal(newSearchText);
	}

	public void ToggleFilter(TaskFilter filter)
	{
		var newFilter = Filter ^ filter;
		newFilter = EnsureFilterPairs(newFilter);
		SetFilterInternal(newFilter);
	}

	public void SetFilter(TaskFilter filter)
	{
		var newFilter = Filter | filter;
		newFilter = EnsureFilterPairs(newFilter);

		SetFilterInternal(newFilter);
	}

	public void UnsetFilter(TaskFilter filter)
	{
		var newFilter = Filter = ~filter;
		newFilter = EnsureFilterPairs(newFilter);

		SetFilterInternal(newFilter);
	}

	public IEnumerable<SpotifyBackgroundTask> Apply(IEnumerable<SpotifyBackgroundTask> source)
	{
		var query = ApplyFilter(source);
		query = ApplySearch(query);
		return query;
	}

	private IEnumerable<SpotifyBackgroundTask> ApplyFilter(IEnumerable<SpotifyBackgroundTask> source)
	{
		if (Filter.HasFlag(_defaultFilter))
		{
			return source;
		}

		var query = source;

		foreach (var pair in FilterPairs)
		{
			query = ApplyFilterPair(query, pair);
		}

		return query;
	}

	private IEnumerable<SpotifyBackgroundTask> ApplyFilterPair(IEnumerable<SpotifyBackgroundTask> source, (TaskFilter A, TaskFilter B) pair)
	{
		bool hasA = Filter.HasFlag(pair.A);
		bool hasB = Filter.HasFlag(pair.B);

		if (hasA && !hasB)
		{
			return source.Where(FilterPredicates[pair.A]);
		}
		if (hasB && !hasA)
		{
			return source.Where(FilterPredicates[pair.B]);
		}

		return source;
	}

	private IEnumerable<SpotifyBackgroundTask> ApplySearch(IEnumerable<SpotifyBackgroundTask> source)
	{
		if (SearchText.IsNullOrEmpty())
		{
			return source;
		}
		var query = source.Where(t => (t.Name?.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase) ?? false) || (t.Status?.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase) ?? false));

		return query;
	}

	private static string? EnsudeSearchText(string? searchText)
	{
		return searchText.IsNullOrEmpty() ? null : searchText.Trim();
	}

	private static TaskFilter EnsureFilterPairs(TaskFilter newFilter)
	{
		foreach (var (pairA, pairB) in FilterPairs)
		{
			var flagA = newFilter.HasFlag(pairA);
			var flagB = newFilter.HasFlag(pairB);

			// none of pair filter is active -> turn on both of them
			if (!flagA && !flagB)
			{
				newFilter |= pairA | pairB;
			}
		}
		return newFilter;
	}

	public bool IsActive(TaskFilter filter)
	{
		return Filter.HasFlag(filter);
	}

	public void ClearFilter()
	{
		SetFilterInternal(_defaultFilter);
	}
}
