using JakubKastner.MusicReleases.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyFilterServiceOld
	{
		Task Delete(string userId);
		Task<SpotifyReleaseFilter?> Get(string userId);
		Task Save(SpotifyReleaseFilter filter, string userId);
	}
}