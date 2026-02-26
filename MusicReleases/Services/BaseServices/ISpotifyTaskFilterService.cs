using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyTaskFilterService
{
	string? SearchText { get; }

	event Action? OnFilterChanged;

	IEnumerable<SpotifyBackgroundTask> Apply(IEnumerable<SpotifyBackgroundTask> source);
	void SetFilter(TaskFilter filter);
	void SetFilterAndSearch(TaskFilter filter, string? searchText);
	void SetSearch(string searchText);
}