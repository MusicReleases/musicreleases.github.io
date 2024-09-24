using IndexedDB.Blazor;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseUpdateController(IIndexedDbFactory dbFactory) : IDatabaseUpdateController
{
	private readonly IIndexedDbFactory _dbFactory = dbFactory;

	public SpotifyLastUpdateEntity GetOrCreate(string userId, SpotifyReleasesDb db)
	{
		var userUpdate = Get(userId, db);

		if (userUpdate is not null)
		{
			return userUpdate;
		}

		userUpdate = new()
		{
			UserId = userId,
		};

		db.Updates.Add(userUpdate);

		return userUpdate;
	}

	public SpotifyLastUpdateEntity? Get(string userId, SpotifyReleasesDb db)
	{
		var userUpdate = db.Updates.SingleOrDefault(x => x.UserId == userId);

		return userUpdate;
	}

	public async Task Delete(string userId)
	{
		using var db = await _dbFactory.Create<SpotifyReleasesDb>();

		var userUpdateDb = Get(userId, db);

		if (userUpdateDb is null)
		{
			return;
		}

		db.Updates.Remove(userUpdateDb);

		await db.SaveChanges();
	}
}
