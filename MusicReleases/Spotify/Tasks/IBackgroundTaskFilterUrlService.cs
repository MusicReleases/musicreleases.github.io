using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskFilterUrlService
{
	string CreateUrlParams(TaskFilter filter, string? searchText);
	TaskFilter ParseFilterFromUrlParams(string? filterParams);
}