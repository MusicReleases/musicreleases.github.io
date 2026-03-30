using JakubKastner.MusicReleases.Spotify.Releases.User;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal interface ISpotifyReleaseFilterUrlService
{
	string CreateUrl(SpotifyReleaseFilter filter);
	SpotifyReleaseFilter ParseFilterFromUrlParams(SpotifyReleaseUrlParameters urlParameters);
}