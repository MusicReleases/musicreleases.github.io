namespace JakubKastner.SpotifyApi.Artists;

public interface ISpotifyArtistClient
{
	Task<IReadOnlyCollection<SpotifyArtist>> GetFollowed(CancellationToken ct = default);
}