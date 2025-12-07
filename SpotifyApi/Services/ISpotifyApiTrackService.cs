using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyApiTrackService
{
	Task<ISet<SpotifyTrack>> GetReleaseTracks(SpotifyRelease release);
}