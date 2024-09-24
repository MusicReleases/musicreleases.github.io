using IndexedDB.Blazor;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseController(IIndexedDbFactory dbFactory) : IDatabaseController
{
	private readonly IIndexedDbFactory _dbFactory = dbFactory;

	public async Task DeleteAll()
	{
		using (var db = await _dbFactory.Create<SpotifyReleasesDb>())
		{
			db.Artists.Clear();
			db.ArtistsReleases.Clear();
			db.Releases.Clear();

			db.Update.Clear();
			db.Users.Clear();

			await db.SaveChanges();
		}
	}
}
