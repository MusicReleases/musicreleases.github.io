using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseArtistReleasesController
	{
		SortedSet<SpotifyRelease> GetReleasesDb(SpotifyReleasesDb db, string artistId);
		void SaveArtistsReleasesDb(SpotifyReleasesDb db, ISet<SpotifyArtist> artists);
		void SaveReleasesDb(SpotifyReleasesDb db, ISet<SpotifyArtist> artists);
	}
}