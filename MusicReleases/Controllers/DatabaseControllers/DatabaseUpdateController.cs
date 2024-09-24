using IndexedDB.Blazor;
using JakubKastner.MusicReleases.Entities.Api.Spotify;

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

		db.Update.Add(userUpdate);

		return userUpdate;
	}

	public SpotifyLastUpdateEntity? Get(string userId, SpotifyReleasesDb db)
	{
		var userUpdate = db.Update.SingleOrDefault(x => x.UserId == userId);

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

		db.Update.Remove(userUpdateDb);

		await db.SaveChanges();
	}
}
