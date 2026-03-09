using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyArtistService
	{
		Task<IReadOnlyList<SpotifyArtist>?> GetAll();
		Task<IReadOnlyList<SpotifyArtist>> GetByIds(IEnumerable<string> ids);
		Task Save(IReadOnlyList<SpotifyArtist> artists);
	}
}