using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyArtistService
	{
		Task<IReadOnlyList<SpotifyArtist>?> GetAll();
		Task<IReadOnlyList<SpotifyArtist>> GetByIds(IEnumerable<string> ids);
		Task Save(IReadOnlyList<SpotifyArtist> artists);
	}
}