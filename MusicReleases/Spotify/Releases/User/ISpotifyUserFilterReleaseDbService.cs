using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Spotify.Releases.User;

internal interface ISpotifyUserFilterReleaseDbService
{
	Task<SpotifyReleaseFilter?> Get(CancellationToken ct);
	Task Save(SpotifyReleaseFilter filter, bool keepExisting, CancellationToken ct);
}