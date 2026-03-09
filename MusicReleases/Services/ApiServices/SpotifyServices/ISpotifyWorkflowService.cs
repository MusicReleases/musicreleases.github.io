using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyWorkflowService
{
	Task StartLoadingAll(ReleaseGroup releaseType, bool forceUpdate);
	Task StartLoadingPlaylistsWithTracks(bool forceUpdate);
	Task StartLoadingArtistsWithReleases(ReleaseGroup releaseType, bool forceUpdate);
	Task StartLoadingReleases(ReleaseGroup releaseType, bool forceUpdate);
	Task Update(UpdateButtonComponent updateType, ReleaseGroup releaseType);
}