using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyTaskFilterUrlService
{
	string CreateUrlParams(TaskFilter filter, string? searchText);
	TaskFilter ParseFilterFromUrlParams(string? filterParams);
	string ToggleInQuery(string? currentQuery, TaskFilter toToggle, string searchText);
}