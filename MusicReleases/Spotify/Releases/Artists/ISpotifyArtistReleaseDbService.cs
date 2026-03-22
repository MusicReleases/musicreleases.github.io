using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Links;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases.Artists;

internal interface ISpotifyArtistReleaseDbService : ISpotifyArtistLinkEntityService<SpotifyArtistReleaseEntity>
{
	Task<IReadOnlyCollection<SpotifyArtistGroupByReleasePayload>> GetArtistsByArtistIds(IReadOnlyCollection<string> artistIds, ReleaseGroup releaseGroup, CancellationToken ct);
}