using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public interface ISpotifyWorkflowController
{
	Task StartLoadingAll(bool forceUpdate, ReleaseType releaseType);
	Task StartLoadingPlaylistsWithTracks(bool forceUpdate);
	Task StartLoadingArtistsWithReleases(bool forceUpdate, ReleaseType releaseType);
	Task StartLoadingReleases(bool forceUpdate, ReleaseType releaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> artists);
}