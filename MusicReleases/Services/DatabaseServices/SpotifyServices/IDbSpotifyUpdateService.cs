using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUpdateService
{
	Task Delete(string userId);
	Task<SpotifyLastUpdateEntity?> Get(string userId);
	Task<SpotifyLastUpdateEntity> GetOrCreate(string userId);
	Task Update(SpotifyLastUpdateEntity lastUpdateEntity);
}