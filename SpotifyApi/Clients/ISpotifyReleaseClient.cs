namespace JakubKastner.SpotifyApi.Clients;

public interface ISpotifyReleaseClient
{
	Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseEnums releaseType, CancellationToken ct = default);
	Task<List<SpotifyRelease>> GetByArtists(IEnumerable<SpotifyArtist> artistIds, ReleaseEnums releaseType, CancellationToken ct = default);
}