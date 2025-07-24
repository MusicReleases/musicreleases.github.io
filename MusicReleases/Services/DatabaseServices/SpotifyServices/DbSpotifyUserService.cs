using DexieNET;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserService(IDexieNETService<SpotifyUserDb> dexieService, /*IDbSpotifyService dbService,*/ IDbSpotifyUpdateService dbUpdateService, IDbSpotifyUserArtistService dbUserArtistService) : IDbSpotifyUserService
{
	//private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyUser);


	private readonly IDexieNETService<SpotifyUserDb> _dexieService = dexieService;
	private SpotifyUserDb? _db;


	private readonly IDbSpotifyUpdateService _dbUpdateService = dbUpdateService;
	private readonly IDbSpotifyUserArtistService _dbUserArtistService = dbUserArtistService;

	private async Task<SpotifyUserDb> InitDb()
	{
		if (_db is null)
		{
			_db = await _dexieService.DexieNETFactory.Create();
			_db.Version(1).Stores();
		}

		return _db;
	}


	public async Task<SpotifyUser?> Get(string userId)
	{
		var updateEntity = await GetDb(userId);
		if (updateEntity is null)
		{
			return null;
		}

		var info = new SpotifyUserInfo()
		{
			Id = updateEntity.Entity.SpotifyId,
			Name = updateEntity.Entity.Name,
			Country = updateEntity.Entity.Country,
			ProfilePictureUrl = updateEntity.Entity.ProfilePictureUrl,
			LastUpdate = updateEntity.Update,
		};
		var credentials = new SpotifyUserCredentials(updateEntity.Entity.RefreshToken);

		var user = new SpotifyUser(info, credentials);
		return user;
	}

	private async Task<SpotifyUpdateDbObject<SpotifyUserEntity2>?> GetDb(string userId)
	{
		var sw = Stopwatch.StartNew();
		Console.WriteLine("db: get user - start");

		var db = await InitDb();
		// get user db
		var userDb = await db.SpotifyUserEntity2s.Get(userId);
		//var userDb = await _dbTable.GetItemAsync<SpotifyUserEntity>(userId);

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
		var updateEntity = new SpotifyUpdateDbObject<SpotifyUserEntity2>(userDb, updateDbUser.Value);
		sw.Stop();
		Console.WriteLine("db: get user - " + sw.ElapsedMilliseconds);
		return updateEntity;
	}

	public async Task Save(SpotifyUser user)
	{
		var sw = Stopwatch.StartNew();
		Console.WriteLine("db: save user - start");
		if (user?.Info is null || user.Credentials is null)
		{
			throw new NullReferenceException(nameof(user));
		}

		var db = await InitDb();
		// user db
		var userDb = new SpotifyUserEntity2(user.Info, user.Credentials.RefreshToken);
		await db.SpotifyUserEntity2s.Put(userDb);

		//await _dbTable.StoreItemAsync(userDb);

		// update db
		var updateDb = await _dbUpdateService.GetOrCreate(user.Info.Id);
		updateDb.User = user.Info.LastUpdate;
		await _dbUpdateService.Update(updateDb);
		sw.Stop();
		Console.WriteLine("db: save user - " + sw.ElapsedMilliseconds);
	}

	public async Task Delete(string userId)
	{
		var sw = Stopwatch.StartNew();
		Console.WriteLine("db: delete user - start");
		var db = await InitDb();
		await db.SpotifyUserEntity2s.Delete(userId);
		//await _dbTable.RemoveItemAsync(userId);
		sw.Stop();
		Console.WriteLine("db: delete user - " + sw.ElapsedMilliseconds);
	}

	public async Task DeleteAllUserDatabases(string userId)
	{
		// TODO delete playlist db
		// delete all databases with this user id
		await Delete(userId);
		await _dbUpdateService.Delete(userId);
		await _dbUserArtistService.Delete(userId);
	}
}
