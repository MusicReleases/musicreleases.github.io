using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyReleaseFilterService
	{
		Task Delete(string userId);
		Task<SpotifyReleaseFilter?> Get(string userId);
		Task Save(SpotifyReleaseFilter filter, string userId);
	}
}