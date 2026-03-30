namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class BackgroundTaskFilterService : IBackgroundTaskFilterService
{
	public event Action? OnFilterChanged;

	private IReadOnlyList<BackgroundTask> _source = [];

	public IReadOnlyList<BackgroundTask> Filtered { get; private set; } = [];

	public string? SearchText { get; private set; } = null;

	public TaskFilter Filter { get; private set; } = _defaultFilter;


	public bool IsFilterActive => !IsActive(_defaultFilter);

	public bool IsSearching => SearchText.IsNotNullOrEmpty();


	private const TaskFilter _defaultFilter = TaskFilter.All;

	private static readonly TaskFilter[] FilterGroup = [TaskFilter.Running, TaskFilter.Canceled, TaskFilter.Failed, TaskFilter.Finished];

	public void SetSource(IReadOnlyList<BackgroundTask> tasks)
	{
		//_source = tasks.Where(t => t.Steps.Any(x => x.Outcome != BackgroundStepOutcome.Skipped)).ToList();
		_source = tasks;
		Console.WriteLine("set source!!!");
		Apply();
	}

	private void SetFilterAndSearchInternal(TaskFilter newFilter, string? newSearchText)
	{
		if (newFilter == Filter && string.Equals(newSearchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		Filter = newFilter;
		SearchText = newSearchText;

		Apply();
		OnFilterChanged?.Invoke();
	}

	private void SetFilterInternal(TaskFilter newFilter)
	{
		if (newFilter == Filter)
		{
			return;
		}
		Filter = newFilter;

		Apply();
		OnFilterChanged?.Invoke();
	}

	private void SetSearchInternal(string? newSearchText)
	{
		if (string.Equals(newSearchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		SearchText = newSearchText;

		Apply();
		OnFilterChanged?.Invoke();
	}

	public void SetFilterAndSearch(TaskFilter filter, string? searchText)
	{
		var newFilter = EnsureFilter(filter);
		var newSearchText = searchText.EnsureText();

		SetFilterAndSearchInternal(newFilter, newSearchText);
	}

	public void SetSearch(string? searchText)
	{
		var newSearchText = searchText.EnsureText();
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

	private void Apply()
	{
		Console.WriteLine("Apply!!!");
		var query = ApplyFilter(_source);
		Filtered = ApplySearch(query).ToList().AsReadOnly();
	}

	private IEnumerable<BackgroundTask> ApplyFilter(IEnumerable<BackgroundTask> source)
	{
		var query = source; //.Where(x => !x.IsWorkflow);

		if (!IsFilterActive)
		{
			return query;
		}

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
		var query = source.ApplySearch
		(
			SearchText,
			t => t.Name,
			t => t.Description,
			t => t.StatusText,
			t => string.Join(" ", t.Steps.Select(s => s.Name)),
			t => string.Join(" ", t.Steps.Select(s => s.Status.ToFriendlyString()))
		);

		return query;
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
