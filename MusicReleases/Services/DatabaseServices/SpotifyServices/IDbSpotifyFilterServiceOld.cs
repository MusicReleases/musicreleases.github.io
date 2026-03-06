using JakubKastner.MusicReleases.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyFilterServiceOld
	{
		Task Delete(string userId);
		Task<SpotifyFilter?> Get(string userId);
		Task Save(SpotifyFilter filter, string userId);
	}
}