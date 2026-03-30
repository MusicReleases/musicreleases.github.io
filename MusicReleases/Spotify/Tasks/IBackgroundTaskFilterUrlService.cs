namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal interface IBackgroundTaskFilterUrlService
{
	string CreateUrlParams(BackgroundTaskFilterType filter, string? searchText);
	BackgroundTaskFilterType ParseFilterFromUrlParams(string? filterParams);
}