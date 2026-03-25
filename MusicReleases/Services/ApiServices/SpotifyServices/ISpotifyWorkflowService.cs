using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyWorkflowService
{
	Task StartLoadingAll(ReleaseGroup releaseType, bool forceUpdate);
	Task Update(UpdateButtonComponent updateType, ReleaseGroup releaseType);
}