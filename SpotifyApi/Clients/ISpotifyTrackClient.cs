namespace JakubKastner.SpotifyApi.Clients;

public interface ISpotifyTrackClient
{
	Task<ISet<SpotifyTrack>> GetReleaseTracks(SpotifyRelease release, CancellationToken ct = default);
}