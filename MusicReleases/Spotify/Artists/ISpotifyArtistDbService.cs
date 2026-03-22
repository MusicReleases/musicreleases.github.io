using JakubKastner.MusicReleases.Database.Spotify.IdEntities;
using JakubKastner.MusicReleases.Spotify.Artists.User;
using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal interface ISpotifyArtistDbService : ISpotifyIdEntityService<SpotifyArtist, SpotifyUserArtistPayload>
{
	Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct);
}