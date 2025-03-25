using IndexedDB.Blazor;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseControllerOld(IIndexedDbFactory dbFactory) : IDatabaseControllerOld
{
	private readonly IIndexedDbFactory _dbFactory = dbFactory;

	public async Task DeleteAll()
	{
		using var db = await _dbFactory.Create<SpotifyReleasesDbOld>();

		db.Users.Clear();
		db.Updates.Clear();

		db.UsersArtists.Clear();
		db.ArtistsReleases.Clear();

		db.Artists.Clear();
		db.Releases.Clear();
		db.Tracks.Clear();

		await db.SaveChanges();
	}
}
