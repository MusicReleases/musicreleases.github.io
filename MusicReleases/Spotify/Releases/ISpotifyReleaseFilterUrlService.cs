using JakubKastner.MusicReleases.Spotify.Releases.User;

namespace JakubKastner.MusicReleases.Spotify.Releases;

public interface ISpotifyReleaseFilterUrlService
{
	string CreateUrl(SpotifyReleaseFilter filter);
	SpotifyReleaseFilter ParseFilterFromUrlParams(string? releaseTypeParam, string? yearParam, string? monthParam, string? artistParam, string? advancedFilterParams, string? searchTextParam);
}