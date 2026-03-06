using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ISpotifyReleaseFilterUrlService
	{
		string CreateUrl(MainReleasesType releaseType, string? year, string? month, string? artist, ReleaseAdvancedFilter advancedFilter, string? searchText);
		string CreateUrl(SpotifyFilter filter);
		SpotifyFilter ParseFilterFromUrlParams(string? releaseType, string? year, string? month, string? artist, string? advancedFilterParams);
	}
}