using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyReleaseService
	{
		Task Get(ReleaseGroup releaseType, bool forceUpdate = false);
	}
}