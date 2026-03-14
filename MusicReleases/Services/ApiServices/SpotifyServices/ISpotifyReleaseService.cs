using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyReleaseService
	{
		Task Get(ReleaseEnums releaseType, bool forceUpdate = false);
	}
}