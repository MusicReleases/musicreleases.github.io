using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public interface ISpotifyWorkflowController
{
	Task StartLoadingAll(bool forceUpdate, ReleaseType releasesType);
	Task StartLoadingPlaylistsWithTracks(bool forceUpdate);
	Task StartLoadingArtistsWithReleases(bool forceUpdate, ReleaseType releasesType);
	Task StartLoadingArtistsReleases(bool forceUpdate, ReleaseType releasesType);
}