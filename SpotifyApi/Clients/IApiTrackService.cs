using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Clients;

internal interface IApiTrackService
{
	Task<ISet<SpotifyTrack>> GetReleaseTracksFromApi(SpotifyRelease release);
}