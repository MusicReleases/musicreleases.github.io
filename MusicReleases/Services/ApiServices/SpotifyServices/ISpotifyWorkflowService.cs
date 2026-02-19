using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyWorkflowService
{
	Task StartLoadingAll(bool forceUpdate, ReleaseType releaseType);
	Task StartLoadingPlaylistsWithTracks(bool forceUpdate);
	Task StartLoadingArtistsWithReleases(bool forceUpdate, ReleaseType releaseType);
	Task StartLoadingReleases(bool forceUpdate, ReleaseType releaseType, ISet<SpotifyArtist> artists);
	Task Update(UpdateButtonComponent updateType, ReleaseType releaseType);
}