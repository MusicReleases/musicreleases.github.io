using IndexedDB.Blazor;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseUserController(IIndexedDbFactory dbFactory, IDatabaseUpdateController databaseUpdateController) : IDatabaseUserController
{
	private readonly IIndexedDbFactory _dbFactory = dbFactory;
	private readonly IDatabaseUpdateController _databaseUpdateController = databaseUpdateController;

	public async Task<SpotifyUser?> Get(string userId)
	{
		var dbEntity = await GetDb(userId);
		if (dbEntity is null || dbEntity.Entity.Id.IsNullOrEmpty() || dbEntity.Entity.Name.IsNullOrEmpty() || dbEntity.Entity.Country.IsNullOrEmpty() || dbEntity.Entity.RefreshToken.IsNullOrEmpty())
		{
			return null;
		}

		var info = new SpotifyUserInfo()
		{
			Id = dbEntity.Entity.Id,
			Name = dbEntity.Entity.Name,
			Country = dbEntity.Entity.Country,
			ProfilePictureUrl = dbEntity.Entity.ProfilePictureUrl,
			LastUpdate = dbEntity.Update,
		};
		var credentials = new SpotifyUserCredentials(dbEntity.Entity.RefreshToken);

		var user = new SpotifyUser(info, credentials);
		return user;
	}

	private async Task<SpotifyUpdateEntity<SpotifyUserEntity>?> GetDb(string userId)
	{
		var userDb = await GetDbBase(userId);

		if (userDb is null)
		{
			// delete all databases with this user
			await DeleteAllDatabases(userId);
			return null;
		}

		using var db = await _dbFactory.Create<SpotifyReleasesDb>();
		var updateDb = _databaseUpdateController.Get(db, userId);
		var userUpdateDb = updateDb?.User;

		if (!userUpdateDb.HasValue)
		{
			// delete all databases with this user
			await DeleteAllDatabases(userId);
			return null;
		}

		return new(userDb, userUpdateDb.Value);
	}

	private async Task<SpotifyUserEntity?> GetDbBase(string userId)
	{
		using var db = await _dbFactory.Create<SpotifyReleasesDb>();

		var userDb = db.Users.SingleOrDefault(x => x.Id == userId);

		return userDb;
	}

	public async Task Save(SpotifyUser user)
	{
		if (user?.Info is null || user.Credentials is null)
		{
			throw new NullReferenceException(nameof(user));
		}

		// old user
		await Delete(user.Info.Id);

		// user db
		var userDb = new SpotifyUserEntity(user.Info, user.Credentials.RefreshToken);
		using var db = await _dbFactory.Create<SpotifyReleasesDb>();
		db.Users.Add(userDb);

		// update db
		var updateDb = _databaseUpdateController.GetOrCreate(db, user.Info.Id);
		updateDb.User = user.Info.LastUpdate;

		await db.SaveChanges();
	}

	public async Task Delete(string userId)
	{
		var userDb = await GetDbBase(userId);

		if (userDb is null)
		{
			return;
		}

		using var db = await _dbFactory.Create<SpotifyReleasesDb>();
		db.Users.Remove(userDb);

		await db.SaveChanges();
	}

	public async Task DeleteAllDatabases(string userId)
	{
		// delete all databases with this user id
		await Delete(userId);
		await _databaseUpdateController.Delete(userId);
	}
}
