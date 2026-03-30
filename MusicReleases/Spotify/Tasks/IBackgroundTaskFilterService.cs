namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskFilterService
{
	string? SearchText { get; }
	TaskFilter Filter { get; }
	bool IsFilterActive { get; }
	bool IsSearching { get; }
	IReadOnlyList<BackgroundTask> Filtered { get; }

	event Action? OnFilterChanged;

	void ClearFilter();
	bool IsActive(TaskFilter filter);
	void SetFilter(TaskFilter filter);
	void SetFilterAndSearch(TaskFilter filter, string? searchText);
	void SetSearch(string searchText);
	void SetSource(IReadOnlyList<BackgroundTask> tasks);
	void ToggleFilter(TaskFilter filter);
	void UnsetFilter(TaskFilter filter);
}