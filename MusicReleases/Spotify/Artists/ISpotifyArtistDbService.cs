using JakubKastner.MusicReleases.Spotify.Artists.User;
using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal interface ISpotifyArtistDbService
{
	Task<SpotifyArtist?> GetById(SpotifyUserArtistPayload payload, CancellationToken ct);
	Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<SpotifyUserArtistPayload> payloads, CancellationToken ct);
	Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct);
	Task Save(IReadOnlyCollection<SpotifyArtist> entities, bool keepExisting, CancellationToken ct);
}