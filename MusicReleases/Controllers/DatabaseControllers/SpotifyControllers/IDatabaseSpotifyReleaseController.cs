using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseSpotifyReleaseController
	{
		Task<ISet<SpotifyRelease>?> Get(ISet<SpotifyReleaseArtistsDbObject> releaseIds);
		Task Save(ISet<SpotifyRelease> releases);
		Task Delete(string releaseId);
	}
}