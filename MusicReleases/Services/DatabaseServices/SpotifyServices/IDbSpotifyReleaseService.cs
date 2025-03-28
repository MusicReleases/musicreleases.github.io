using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyReleaseService
{
	Task<ISet<SpotifyRelease>?> Get(ISet<SpotifyReleaseArtistsDbObject> releaseIds);
	Task Save(ISet<SpotifyRelease> releases);
	Task Delete(string releaseId);
}