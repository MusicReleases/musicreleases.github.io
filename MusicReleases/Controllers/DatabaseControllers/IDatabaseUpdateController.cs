using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseUpdateController
{
	Task Delete(string userId);
	SpotifyLastUpdateEntity? Get(string userId, SpotifyReleasesDb db);
	SpotifyLastUpdateEntity GetOrCreate(string userId, SpotifyReleasesDb db);
}