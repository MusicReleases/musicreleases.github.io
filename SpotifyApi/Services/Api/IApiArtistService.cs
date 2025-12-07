using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

public interface IApiArtistService
{
	Task<List<SpotifyArtist>> GetFollowed(CancellationToken ct = default);
}