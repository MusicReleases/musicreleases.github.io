namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskFilterService
{
	string? SearchText { get; }
	BackgroundTaskFilterType Filter { get; }
	bool IsFilterActive { get; }
	bool IsSearching { get; }
	IReadOnlyList<BackgroundTask> Filtered { get; }

	event Action? OnFilterChanged;

	void ClearFilter();
	bool IsActive(BackgroundTaskFilterType filter);
	void SetFilter(BackgroundTaskFilterType filter);
	void SetFilterAndSearch(BackgroundTaskFilterType filter, string? searchText);
	void SetSearch(string searchText);
	void SetSource(IReadOnlyList<BackgroundTask> tasks);
	void ToggleFilter(BackgroundTaskFilterType filter);
	void UnsetFilter(BackgroundTaskFilterType filter);
}