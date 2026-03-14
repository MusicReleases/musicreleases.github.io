using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyWorkflowService
{
	Task StartLoadingAll(ReleaseEnums releaseType, bool forceUpdate);
	Task StartLoadingPlaylistsWithTracks(bool forceUpdate);
	Task StartLoadingArtistsWithReleases(ReleaseEnums releaseType, bool forceUpdate);
	Task StartLoadingReleases(ReleaseEnums releaseType, bool forceUpdate);
	Task Update(UpdateButtonComponent updateType, ReleaseEnums releaseType);
}