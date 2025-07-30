using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiTrackService
{
	Task<ISet<SpotifyTrack>> GetReleaseTracksFromApi(SpotifyRelease release);
}