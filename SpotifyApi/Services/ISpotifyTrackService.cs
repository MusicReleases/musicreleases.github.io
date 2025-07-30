using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyTrackService
{
	Task<ISet<SpotifyTrack>> GetReleaseTracks(SpotifyRelease release);
}