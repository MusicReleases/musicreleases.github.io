using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public interface IDatabaseSpotifyUpdateController
{
	Task Delete(string userId);
	Task<SpotifyLastUpdateEntity?> Get(string userId);
	Task<SpotifyLastUpdateEntity> GetOrCreate(string userId);
	Task Update(SpotifyLastUpdateEntity lastUpdateEntity);
}