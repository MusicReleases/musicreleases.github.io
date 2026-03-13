using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public interface IDbSpotifyUserUpdateService
{
	Task Delete(string userId, SpotifyDbUpdateType updateType);
	Task<DateTime> Get(string userId, SpotifyDbUpdateType dbType, CancellationToken ct);
	Task Save(string userId, SpotifyDbUpdateType dbType, CancellationToken ct = default);
}