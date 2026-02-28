using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyTaskFilterService
{
	string? SearchText { get; }
	TaskFilter Filter { get; }
	bool IsFilterActive { get; }

	event Action? OnFilterChanged;

	IEnumerable<SpotifyBackgroundTask> Apply(IEnumerable<SpotifyBackgroundTask> source);
	void ClearFilter();
	bool IsActive(TaskFilter filter);
	void SetFilter(TaskFilter filter);
	void SetFilterAndSearch(TaskFilter filter, string? searchText);
	void SetSearch(string searchText);
	void ToggleFilter(TaskFilter filter);
	void UnsetFilter(TaskFilter filter);
}