using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers
{
	public interface IDatabaseReleasesController
	{
		Task<SortedSet<SpotifyRelease>> GetReleasesDb(IndexedDb db, string artistId, bool getReleases);
		Task SaveArtistsReleasesDb(IndexedDb db, ISet<SpotifyArtist> artists);
		Task SaveReleasesDb(IndexedDb db, ISet<SpotifyArtist> artists);
	}
}