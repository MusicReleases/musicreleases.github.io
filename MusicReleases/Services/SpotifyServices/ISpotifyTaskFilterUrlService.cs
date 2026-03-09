using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public interface ISpotifyTaskFilterUrlService
{
	string CreateUrlParams(TaskFilter filter, string? searchText);
	TaskFilter ParseFilterFromUrlParams(string? filterParams);
}