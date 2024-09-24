using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseUpdateController
{
	Task Delete(string userId);
	SpotifyLastUpdateEntity? Get(SpotifyReleasesDb db, string userId);
	SpotifyLastUpdateEntity GetOrCreate(SpotifyReleasesDb db, string userId);
}