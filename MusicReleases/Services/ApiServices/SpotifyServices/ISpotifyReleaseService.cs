using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyReleaseService
	{
		Task Get(ReleaseGroup releaseType, bool forceUpdate = false);
	}
}