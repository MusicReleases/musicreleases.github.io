using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyReleaseService
	{
		void Cancel();
		Task Get(MainReleasesType releaseType, bool forceUpdate = false);
	}
}