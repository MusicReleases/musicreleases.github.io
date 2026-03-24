using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Spotify.User.Update;

public interface ISpotifyUserUpdateDbService
{
	Task Delete(string userId, SpotifyDbUpdateType updateType);
	Task DeleteForUser(string userId);
	Task<DateTime> Get(string userId, SpotifyDbUpdateType dbType, CancellationToken ct = default);
	Task Save(string userId, SpotifyDbUpdateType dbType, CancellationToken ct = default);
}