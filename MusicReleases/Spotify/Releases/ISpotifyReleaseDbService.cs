using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases
{
	internal interface ISpotifyReleaseDbService
	{
		Task<IReadOnlyCollection<SpotifyRelease>> GetByIds(IReadOnlyCollection<SpotifyArtistReleasePayload> payloads, ReleaseGroup releaseGroup, CancellationToken ct);
		Task Save(
			IReadOnlyCollection<SpotifyRelease> releases,
			bool keepExisting,
			CancellationToken ct);

	}
}