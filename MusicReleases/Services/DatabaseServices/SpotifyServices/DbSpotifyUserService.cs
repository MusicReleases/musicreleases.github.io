using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserService(IDbSpotifyServiceOld dbService, IDbSpotifyUpdateService dbUpdateService, IDbSpotifyUserArtistService dbUserArtistService, IDbSpotifyUserPlaylistService dbUserPlaylistService, IDbSpotifyFilterService dbFilterService) : IDbSpotifyUserService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyUser);

	private readonly IDbSpotifyUpdateService _dbUpdateService = dbUpdateService;
	private readonly IDbSpotifyUserArtistService _dbUserArtistService = dbUserArtistService;
	private readonly IDbSpotifyUserPlaylistService _dbUserPlaylistService = dbUserPlaylistService;
	private readonly IDbSpotifyFilterService _dbFilterService = dbFilterService;

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
		Console.WriteLine("dB: get user - start");
		var userDb = await _dbTable.GetItemAsync<SpotifyUserEntity>(userId);
		Console.WriteLine("dB: get user - end");

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
		Console.WriteLine("db: save user - start");
		var userDb = new SpotifyUserEntity(user.Info, user.Credentials.RefreshToken);
		await _dbTable.StoreItemAsync(userDb);
		Console.WriteLine("db: save user - end");

		// update db
		var updateDb = await _dbUpdateService.GetOrCreate(user.Info.Id);
		updateDb.User = user.Info.LastUpdate;
		await _dbUpdateService.Update(updateDb);
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("db: delete user - start");
		await _dbTable.RemoveItemAsync(userId);
		Console.WriteLine("db: delete user - end");
	}

	public async Task DeleteAllUserDatabases(string userId)
	{
		// TODO delete playlist db
		// delete all databases with this user id
		await Delete(userId);
		await _dbUpdateService.Delete(userId);
		await _dbFilterService.Delete(userId);
		await _dbUserArtistService.Delete(userId);
		await _dbUserPlaylistService.Delete(userId);
	}
}
