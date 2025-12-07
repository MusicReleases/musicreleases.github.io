using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUpdateService
{
	Task<DateTime> Get(string userId, LoadingType dbType);
	Task SetLastArtistSync(string userId, LoadingType dbType);
}