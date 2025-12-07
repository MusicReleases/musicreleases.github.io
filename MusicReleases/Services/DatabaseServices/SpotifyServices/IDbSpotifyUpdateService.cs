using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUpdateService
{
	Task<DateTime> Get(string userId, SpotifyDbUpdateType dbType);
	Task Save(string userId, SpotifyDbUpdateType dbType);
}