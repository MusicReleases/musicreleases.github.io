using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public interface IDbSpotifyUpdateService
{
	Task<DateTime> Get(string userId, SpotifyDbUpdateType dbType);
	Task Save(string userId, SpotifyDbUpdateType dbType);
}