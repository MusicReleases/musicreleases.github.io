using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.SpotifyApi.Tracks;

public interface ISpotifyTrackClient
{
	Task<ISet<SpotifyTrack>> GetReleaseTracks(SpotifyRelease release, CancellationToken ct = default);
}