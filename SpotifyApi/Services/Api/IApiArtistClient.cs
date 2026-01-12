using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

public interface IApiArtistClient
{
	Task<List<SpotifyArtist>> GetFollowed(CancellationToken ct = default);
}