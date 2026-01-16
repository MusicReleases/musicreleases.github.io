using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyFilterUrlService
{
	Task<string> ClearFilter(MenuType type);
	Task<string> GetFilterUrl();
	Task<string> GetFilterUrl(int? year);
	Task<string> GetFilterUrl(int? year, int? month);
	Task<string> GetFilterUrl(ReleaseType releaseType);
	Task<string> GetFilterUrl(string? artist);
	Task<string> GetFilterUrl(ReleasesFilters advancedFilterType, bool advancedFilterActive);
	SpotifyFilter ParseFilterUrl(string? releaseType, string? year, string? month, string? artist, SpotifyFilterAdvanced advancedFilter);
}