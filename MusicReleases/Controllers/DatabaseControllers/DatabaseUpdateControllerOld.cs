using IndexedDB.Blazor;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseUpdateControllerOld(IIndexedDbFactory dbFactory) : IDatabaseUpdateControllerOld
{
	private readonly IIndexedDbFactory _dbFactory = dbFactory;

	public SpotifyLastUpdateEntity GetOrCreate(SpotifyReleasesDbOld db, string userId)
	{
		var userUpdate = Get(db, userId);

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

	public SpotifyLastUpdateEntity? Get(SpotifyReleasesDbOld db, string userId)
	{
		var userUpdate = db.Updates.SingleOrDefault(x => x.UserId == userId);
		return userUpdate;
	}

	public async Task Delete(string userId)
	{
		using var db = await _dbFactory.Create<SpotifyReleasesDbOld>();

		var userUpdateDb = Get(db, userId);

		if (userUpdateDb is null)
		{
			return;
		}

		db.Updates.Remove(userUpdateDb);

		await db.SaveChanges();
	}
}
