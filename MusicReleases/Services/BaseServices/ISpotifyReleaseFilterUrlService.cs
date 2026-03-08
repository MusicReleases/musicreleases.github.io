using JakubKastner.MusicReleases.Objects;

namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ISpotifyReleaseFilterUrlService
	{
		string CreateUrl(SpotifyFilter filter);
		SpotifyFilter ParseFilterFromUrlParams(string? releaseTypeParam, string? yearParam, string? monthParam, string? artistParam, string? advancedFilterParams, string? searchTextParam);
	}
}