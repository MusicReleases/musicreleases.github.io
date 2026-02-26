using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyTaskFilterService : ISpotifyTaskFilterService
{
	public event Action? OnFilterChanged;

	public string? SearchText { get; private set; } = null;


	private TaskFilter _filter = TaskFilter.All;


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

	private void NotifyUI()
	{
		OnFilterChanged?.Invoke();
	}

	public void SetFilterAndSearch(TaskFilter filter, string? searchText)
	{
		SetFilterInternal(filter);
		SetSearchInternal(searchText);

		NotifyUI();
	}

	public void SetSearch(string? searchText)
	{
		SetSearchInternal(searchText);
		NotifyUI();
	}

	private void SetSearchInternal(string? searchText)
	{
		SearchText = searchText.IsNullOrEmpty() ? null : searchText.Trim();
	}

	public void SetFilter(TaskFilter filter)
	{
		SetFilterInternal(filter);
		NotifyUI();
	}

	private void SetFilterInternal(TaskFilter filter)
	{
		_filter = EnsurePairsValid(filter);
	}


	public IEnumerable<SpotifyBackgroundTask> Apply(IEnumerable<SpotifyBackgroundTask> source)
	{
		var query = ApplyFilter(source);
		query = ApplySearch(query);
		return query;
	}


	private IEnumerable<SpotifyBackgroundTask> ApplyFilter(IEnumerable<SpotifyBackgroundTask> source)
	{
		if (_filter.HasFlag(TaskFilter.All))
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
		bool hasA = _filter.HasFlag(pair.A);
		bool hasB = _filter.HasFlag(pair.B);

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

	private static TaskFilter EnsurePairsValid(TaskFilter value)
	{
		foreach (var (pairA, pairB) in FilterPairs)
		{
			bool flagA = value.HasFlag(pairA);
			bool flagB = value.HasFlag(pairB);

			if (!flagA && !flagB)
			{
				value |= pairA | pairB;
			}
		}
		return value;
	}
}
