using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseUserController(IDatabaseController dbController, IDatabaseUpdateController databaseUpdateController) : IDatabaseUserController
{
	private readonly IDatabaseController _dbController = dbController;
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
		var db = _dbController.GetDb();
		var userDb = await GetDbBase(db, userId);

		if (userDb is null)
		{
			// delete all databases with this user
			await DeleteAllDatabases(userId);
			return null;
		}

		var updateDb = await _databaseUpdateController.Get(db, userId);
		var userUpdateDb = updateDb?.User;

		if (!userUpdateDb.HasValue)
		{
			// delete all databases with this user
			await DeleteAllDatabases(userId);
			return null;
		}

		return new(userDb, userUpdateDb.Value);
	}

	private async Task<SpotifyUserEntity?> GetDbBase(IndexedDb db, string userId)
	{
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUser);
		var userDb = await table.GetItemAsync<SpotifyUserEntity>(userId);
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

		var db = _dbController.GetDb();

		// user db
		Console.WriteLine("save user");
		var userDb = new SpotifyUserEntity(user.Info, user.Credentials.RefreshToken);
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUser);
		await table.StoreItemAsync(userDb);

		// update db
		Console.WriteLine("save update");
		var updateDb = await _databaseUpdateController.GetOrCreate(db, user.Info.Id);
		updateDb.User = user.Info.LastUpdate;
		var tableUpdate = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUpdate);
		await tableUpdate.StoreItemAsync(updateDb);
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("delete user");
		var db = _dbController.GetDb();
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUser);
		await table.RemoveItemAsync(userId);
	}

	public async Task DeleteAllDatabases(string userId)
	{
		// TODO delete playlist db

		// delete all databases with this user id
		await Delete(userId);
		await _databaseUpdateController.Delete(userId);
	}
}
