using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserArtistServiceOld(IDbSpotifyServiceOld dbService, IDbSpotifyArtistService dbArtistService, IDbSpotifyUpdateServiceOld dbUpdateService) : IDbSpotifyUserArtistServiceOld
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyUserArtist);

	private readonly IDbSpotifyArtistService _dbArtistService = dbArtistService;
	private readonly IDbSpotifyUpdateServiceOld _dbUpdateService = dbUpdateService;

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>?> Get(string userId)
	{
		// update
		var update = await GetUpdateDb(userId);
		if (update is null)
		{
			return null;
		}

		// artists
		var artists = await GetFollowed(userId);
		if (artists.Count < 1)
		{
			return null;
		}

		var artistUpdate = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>(artists, update);

		return artistUpdate;
	}

	private async Task<ISet<SpotifyArtist>> GetFollowed(string userId)
	{
		// get followed artist ids
		var followedArtistDb = await GetFollowedDb(userId);
		var artistIdsDb = followedArtistDb.Select(x => x.ArtistId);

		// get all artists (name) from db
		var artistsDb = await _dbArtistService.GetAll();
		if (artistsDb is null)
		{
			throw new NullReferenceException(nameof(artistsDb));
		}

		var artists = new HashSet<SpotifyArtist>();
		foreach (var artistId in artistIdsDb)
		{
			var artistDb = artistsDb.First(x => x.Id == artistId);
			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name, artistDb.UrlApp, artistDb.UrlWeb);
			artists.Add(artist);
		}

		return artists;
	}


	private async Task<ISet<SpotifyUserArtistEntity>> GetFollowedDb(string userId)
	{
		// TODO user artist db table
		Console.WriteLine("db: get user artists - start");

		// get artists from db
		var userArtistsDb = _dbTable.GetAllAsync<SpotifyUserArtistEntity>();
		var artistsDb = new HashSet<SpotifyUserArtistEntity>();


		await foreach (var userArtistDb in userArtistsDb)
		{
			if (userArtistDb.UserId != userId)
			{
				continue;
			}
			artistsDb.Add(userArtistDb);
		}
		Console.WriteLine("db: get user artists - end");
		return artistsDb;
	}

	private async Task<SpotifyUserListUpdateMain?> GetUpdateDb(string userId)
	{
		var updateDb = await _dbUpdateService.Get(userId);
		var updateDbArtists = updateDb?.Artists;
		if (!updateDbArtists.HasValue)
		{
			// TODO delete user artists
			return null;
		}

		var update = new SpotifyUserListUpdateMain(updateDbArtists.Value);
		return update;
	}

	public async Task Save(string userId, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> artists)
	{
		// TODO remove unfollowed artists and deleted releases

		if (artists.Update is null)
		{
			// TODO
			throw new NullReferenceException(nameof(artists.Update));
		}
		if (artists.List is null)
		{
			// TODO
			throw new NullReferenceException(nameof(artists.List));
		}

		// update db
		await SaveUpdateDb(userId, artists.Update);

		// user artists db
		await SaveDb(artists.List, userId);
	}

	private async Task SaveUpdateDb(string userId, SpotifyUserListUpdateMain update)
	{
		// TODO null
		//var updateDb = await _databaseUpdateController.Get(db, userId);

		var updateDb = await _dbUpdateService.Get(userId);

		if (updateDb is null)
		{
			// TODO
			throw new NullReferenceException(nameof(updateDb));
		}

		// update - update times
		updateDb.Artists = update.LastUpdateMain;
		await _dbUpdateService.Update(updateDb);
	}


	private async Task SaveDb(ISet<SpotifyArtist> artists, string userId)
	{
		var userArtistsDb = await GetFollowedDb(userId);

		var userArtistIds = userArtistsDb.Select(x => x.ArtistId).ToHashSet();
		var artistIds = artists.Select(x => x.Id).ToHashSet();

		var newFollowedArtists = artists.Where(x => !userArtistIds.Contains(x.Id)).ToHashSet();
		var unfollowedArtists = userArtistsDb.Where(x => !artistIds.Contains(x.ArtistId)).ToHashSet();

		// save new followed artists
		await _dbArtistService.Save(newFollowedArtists.ToList());
		Console.WriteLine("db: save user artists - start");
		foreach (var artist in newFollowedArtists)
		{
			var artistEntity = new SpotifyUserArtistEntity(userId, artist.Id);
			await _dbTable.StoreAsync(artistEntity);
		}
		Console.WriteLine("db: save user artists - end");

		// delete not followed artists
		await Delete(unfollowedArtists);
	}

	private async Task Delete(ISet<SpotifyUserArtistEntity> userArtistsDb)
	{
		Console.WriteLine("db: delete user artists - start");
		foreach (var userArtistDb in userArtistsDb)
		{
			await _dbTable.RemoveItemAsync(userArtistDb);
		}
		Console.WriteLine("db: delete user artists - end");
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("db: delete user artists by user id - start");
		var userArtistsDb = _dbTable.GetAllAsync<SpotifyUserArtistEntity>();
		await foreach (var userArtistDb in userArtistsDb)
		{
			if (userArtistDb.UserId != userId)
			{
				continue;
			}
			await _dbTable.RemoveItemAsync(userArtistDb);
		}
		Console.WriteLine("db: delete followed artists by user id - end");
	}
}
