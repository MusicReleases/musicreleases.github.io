using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Spotify.Releases.User
{
	public interface IDbSpotifyUserFilterReleaseService
	{
		Task Delete(string userId);
		Task<SpotifyReleaseFilter?> Get(string userId);
		Task Save(SpotifyReleaseFilter filter, string userId);
	}
}