using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal interface ISpotifyReleaseDbService : ISpotifyIdEntityService<SpotifyRelease, SpotifyArtistReleasePayload>
{
	Task<IReadOnlyCollection<SpotifyRelease>> GetByIds(IReadOnlyCollection<SpotifyArtistReleasePayload> payloads, ReleaseGroup releaseGroup, CancellationToken ct);
}