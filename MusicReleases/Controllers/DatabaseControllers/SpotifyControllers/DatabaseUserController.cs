using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public class DatabaseUserController(IDatabaseController dbController, IDatabaseUpdateController dbUpdateController) : IDatabaseUserController
{
	private readonly IDatabaseUpdateController _dbUpdateController = dbUpdateController;
	private readonly IndexedDbStore _dbTable = dbController.GetTable(DbStorageTablesSpotify.SpotifyUser);

	public async Task<SpotifyUser?> Get(string userId)
	{
		var dbEntity = await GetDb(userId);
		if (dbEntity is null)
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
		var userDb = await _dbTable.GetItemAsync<SpotifyUserEntity>(userId);

		if (userDb is null)
		{
			// delete all databases with this user
			await DeleteAllUserDatabases(userId);
			return null;
		}

		var updateDb = await _dbUpdateController.Get(userId);
		var userUpdateDb = updateDb?.User;

		if (!userUpdateDb.HasValue)
		{
			// delete all databases with this user
			await DeleteAllUserDatabases(userId);
			return null;
		}

		var updateEntity = new SpotifyUpdateEntity<SpotifyUserEntity>(userDb, userUpdateDb.Value);
		return updateEntity;
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
		Console.WriteLine("save user");
		var userDb = new SpotifyUserEntity(user.Info, user.Credentials.RefreshToken);
		await _dbTable.StoreItemAsync(userDb);

		// update db
		var updateDb = await _dbUpdateController.GetOrCreate(user.Info.Id);
		updateDb.User = user.Info.LastUpdate;
		await _dbUpdateController.Update(updateDb);
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("delete user");
		await _dbTable.RemoveItemAsync(userId);
	}

	public async Task DeleteAllUserDatabases(string userId)
	{
		// TODO delete playlist db
		// delete all databases with this user id
		await Delete(userId);
		await _dbUpdateController.Delete(userId);
	}
}
