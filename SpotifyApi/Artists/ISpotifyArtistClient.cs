namespace JakubKastner.SpotifyApi.Artists;

public interface ISpotifyArtistClient
{
	Task<IReadOnlyCollection<SpotifyArtist>> GetFollowed(CancellationToken ct = default);
	IAsyncEnumerable<IReadOnlyCollection<SpotifyArtist>> GetFollowedBatches(int batchSize = 25, CancellationToken ct = default);
}