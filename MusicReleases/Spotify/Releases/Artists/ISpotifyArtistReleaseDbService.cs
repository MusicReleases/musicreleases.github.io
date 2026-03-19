using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases.Artists;

internal interface ISpotifyArtistReleaseDbService : ISpotifyArtistLinkEntityService<SpotifyArtistReleaseEntity>
{
	Task<IReadOnlyCollection<SpotifyArtistGroupPayload>> GetArtistsByArtistIds(IReadOnlyCollection<string> artistIds, ReleaseGroup releaseGroup, CancellationToken ct);
}