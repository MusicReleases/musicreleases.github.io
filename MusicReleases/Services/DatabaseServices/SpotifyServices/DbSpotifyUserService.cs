using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserService(IDbSpotifyService dbService, IDbSpotifyUpdateService dbUpdateService) : IDbSpotifyUserService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyUser);

	private readonly IDbSpotifyUpdateService _dbUpdateService = dbUpdateService;

	public async Task<SpotifyUser?> Get(string userId)
	{
		var updateEntity = await GetDb(userId);
		if (updateEntity is null)
		{
			return null;
		}

		var info = new SpotifyUserInfo()
		{
			Id = updateEntity.Entity.Id,
			Name = updateEntity.Entity.Name,
			Country = updateEntity.Entity.Country,
			ProfilePictureUrl = updateEntity.Entity.ProfilePictureUrl,
			LastUpdate = updateEntity.Update,
		};
		var credentials = new SpotifyUserCredentials(updateEntity.Entity.RefreshToken);

		var user = new SpotifyUser(info, credentials);
		return user;
	}

	private async Task<SpotifyUpdateDbObject<SpotifyUserEntity>?> GetDb(string userId)
	{
		// get user db
		var userDb = await _dbTable.GetItemAsync<SpotifyUserEntity>(userId);

		if (userDb is null)
		{
			// delete all databases with this user
			await DeleteAllUserDatabases(userId);
			return null;
		}

		// get update db
		var updateDb = await _dbUpdateService.Get(userId);
		var updateDbUser = updateDb?.User;

		// user doesnt exist or doesnt have update time
		if (!updateDbUser.HasValue)
		{
			// delete all databases with this user
			await DeleteAllUserDatabases(userId);
			return null;
		}

		// create new update entity
		var updateEntity = new SpotifyUpdateDbObject<SpotifyUserEntity>(userDb, updateDbUser.Value);
		return updateEntity;
	}

	public async Task Save(SpotifyUser user)
	{
		if (user?.Info is null || user.Credentials is null)
		{
			throw new NullReferenceException(nameof(user));
		}

		// user db
		Console.WriteLine("save user");
		var userDb = new SpotifyUserEntity(user.Info, user.Credentials.RefreshToken);
		await _dbTable.StoreItemAsync(userDb);

		// update db
		var updateDb = await _dbUpdateService.GetOrCreate(user.Info.Id);
		updateDb.User = user.Info.LastUpdate;
		await _dbUpdateService.Update(updateDb);
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
		await _dbUpdateService.Delete(userId);
	}
}
