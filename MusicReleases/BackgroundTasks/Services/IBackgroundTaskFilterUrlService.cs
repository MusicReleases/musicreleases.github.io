using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.BackgroundTasks.Services;

internal interface IBackgroundTaskFilterUrlService
{
	string CreateUrlParams(TaskFilter filter, string? searchText);
	TaskFilter ParseFilterFromUrlParams(string? filterParams);
}