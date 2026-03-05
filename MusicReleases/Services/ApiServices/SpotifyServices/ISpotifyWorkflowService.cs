using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyWorkflowService
{
	Task StartLoadingAll(MainReleasesType releaseType, bool forceUpdate);
	Task StartLoadingPlaylistsWithTracks(bool forceUpdate);
	Task StartLoadingArtistsWithReleases(MainReleasesType releaseType, bool forceUpdate);
	Task StartLoadingReleases(MainReleasesType releaseType, bool forceUpdate);
	Task Update(UpdateButtonComponent updateType, MainReleasesType releaseType);
}