using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal interface ISpotifyArtistDbService
{
	Task<IReadOnlyCollection<SpotifyArtist>> GetAll(CancellationToken ct);
	Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct);
	Task Save(IReadOnlyCollection<SpotifyArtist> artists, CancellationToken ct);
}