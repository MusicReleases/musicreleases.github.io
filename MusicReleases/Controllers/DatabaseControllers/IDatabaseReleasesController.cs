using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseReleasesController
	{
		Task<SortedSet<SpotifyRelease>> GetReleasesDb(string artistId, bool getReleases);
		Task SaveArtistsReleasesDb(ISet<SpotifyArtist> artists);
		Task SaveReleasesDb(ISet<SpotifyArtist> artists);
	}
}