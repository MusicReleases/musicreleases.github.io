using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases
{
	public interface ISpotifyReleaseService
	{
		Task Get(ReleaseEnums releaseType, bool forceUpdate = false);
	}
}