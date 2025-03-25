using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseUpdateControllerOld
{
	Task Delete(string userId);
	SpotifyLastUpdateEntity? Get(SpotifyReleasesDbOld db, string userId);
	SpotifyLastUpdateEntity GetOrCreate(SpotifyReleasesDbOld db, string userId);
}