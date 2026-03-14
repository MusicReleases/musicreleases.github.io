namespace JakubKastner.SpotifyApi.Clients;

public interface ISpotifyArtistClient
{
	Task<List<SpotifyArtist>> GetFollowed(CancellationToken ct = default);
}