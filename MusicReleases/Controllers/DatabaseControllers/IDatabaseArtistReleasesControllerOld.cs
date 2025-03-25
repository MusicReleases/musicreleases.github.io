using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseArtistReleasesControllerOld
	{
		SortedSet<SpotifyRelease> GetReleasesDb(SpotifyReleasesDbOld db, string artistId);
		void SaveArtistsReleasesDb(SpotifyReleasesDbOld db, ISet<SpotifyArtist> artists);
		void SaveReleasesDb(SpotifyReleasesDbOld db, ISet<SpotifyArtist> artists);
	}
}