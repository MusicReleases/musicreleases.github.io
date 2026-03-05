using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyReleaseServiceOld
{
	Task<ISet<SpotifyReleaseOld>?> Get(ISet<SpotifyReleaseArtistsDbObject> releaseIds);
	Task Save(ISet<SpotifyReleaseOld> releases);
	Task Delete(string releaseId);
}