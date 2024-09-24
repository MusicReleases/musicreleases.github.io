using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseArtistReleasesController
	{
		SortedSet<SpotifyRelease> GetReleasesDb(SpotifyReleasesDb db, string artistId);
		(ISet<SpotifyArtistReleaseEntity> artistReleases, ISet<SpotifyArtistEntity> artists) SaveArtistsReleasesDb(ISet<SpotifyArtist> artists);
		ISet<SpotifyReleaseEntity> SaveReleasesDb(ISet<SpotifyArtist> artists);
	}
}