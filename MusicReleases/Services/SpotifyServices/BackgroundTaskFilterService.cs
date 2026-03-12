using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class BackgroundTaskFilterService : IBackgroundTaskFilterService
{
	public event Action? OnFilterChanged;

	public string? SearchText { get; private set; } = null;

	public TaskFilter Filter { get; private set; } = _defaultFilter;

	public bool IsFilterActive => Filter != _defaultFilter;

	public bool IsSearching => SearchText.IsNotNullOrEmpty();


	private const TaskFilter _defaultFilter = TaskFilter.All;


	private static readonly TaskFilter[] FilterGroup = [TaskFilter.Running, TaskFilter.Canceled, TaskFilter.Failed, TaskFilter.Finished];


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
		var newFilter = EnsureFilter(filter);
		var newSearchText = EnsureSearchText(searchText);

		SetFilterAndSearchInternal(newFilter, newSearchText);
	}

	public void SetSearch(string? searchText)
	{
		var newSearchText = EnsureSearchText(searchText);
		SetSearchInternal(newSearchText);
	}

	public void ToggleFilter(TaskFilter filter)
	{
		var newFilter = Filter ^ filter;
		newFilter = EnsureFilter(newFilter);
		SetFilterInternal(newFilter);
	}

	public void SetFilter(TaskFilter filter)
	{
		var newFilter = Filter | filter;
		newFilter = EnsureFilter(newFilter);

		SetFilterInternal(newFilter);
	}

	public void UnsetFilter(TaskFilter filter)
	{
		var newFilter = Filter & ~filter;
		newFilter = EnsureFilter(newFilter);

		SetFilterInternal(newFilter);
	}

	public IEnumerable<BackgroundTask> Apply(IEnumerable<BackgroundTask> source)
	{
		var query = ApplyFilter(source);
		query = ApplySearch(query);
		return query;
	}

	private IEnumerable<BackgroundTask> ApplyFilter(IEnumerable<BackgroundTask> source)
	{
		if (IsActive(_defaultFilter))
		{
			return source;
		}

		var query = source;

		var running = IsActive(TaskFilter.Running);
		var canceled = IsActive(TaskFilter.Canceled);
		var failed = IsActive(TaskFilter.Failed);
		var finished = IsActive(TaskFilter.Finished);

		var anySelected = running || canceled || failed || finished;
		var allSelected = running && canceled && failed && finished;

		if (anySelected && !allSelected)
		{
			query = query.Where(t =>
				(running && t.Status == BackgroundTaskStatus.Running) ||
				(canceled && t.Status == BackgroundTaskStatus.Canceled) ||
				(failed && t.Status == BackgroundTaskStatus.Failed) ||
				(finished && t.Status == BackgroundTaskStatus.Finished)
			);
		}
		return query;
	}

	private IEnumerable<BackgroundTask> ApplySearch(IEnumerable<BackgroundTask> source)
	{
		var query = source.ApplySearch(SearchText,
			t => t.Name,
			t => t.Info,
			t => t.StatusText,
			t => string.Join(" ", t.Steps.Select(s => s.Name)),
			t => string.Join(" ", t.Steps.Select(s => s.Status.ToFriendlyString()))
		);

		return query;
	}

	private static string? EnsureSearchText(string? searchText)
	{
		return searchText.IsNullOrEmpty() ? null : searchText.Trim();
	}

	private static TaskFilter EnsureFilter(TaskFilter newFilter)
	{
		var anyGroupActive = newFilter.HasAnyFlag(FilterGroup);

		if (!anyGroupActive)
		{
			return _defaultFilter;
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
