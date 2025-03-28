using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public class DatabaseSpotifyUserArtistController(IDatabaseSpotifyController dbController, IDatabaseSpotifyArtistController dbArtistController, IDatabaseSpotifyUpdateController dbUpdateController) : IDatabaseSpotifyUserArtistController
{
	private readonly IndexedDbStore _dbTable = dbController.GetTable(DbStorageTablesSpotify.SpotifyUserArtist);

	private readonly IDatabaseSpotifyArtistController _dbArtistController = dbArtistController;
	private readonly IDatabaseSpotifyUpdateController _dbUpdateController = dbUpdateController;

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
		var userArtistsDb = _dbTable.GetAllAsync<SpotifyUserArtistEntity>();
		var artistsDb = await _dbArtistController.GetAll();
		if (artistsDb is null)
		{
			throw new NullReferenceException(nameof(artistsDb));
		}

		var artists = new HashSet<SpotifyArtist>();
		foreach (var artistId in artistIdsDb)
		{
			var artistDb = artistsDb.First(x => x.Id == artistId);
			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name);
			artists.Add(artist);
		}

		return artists;
	}


	private async Task<ISet<SpotifyUserArtistEntity>> GetFollowedDb(string userId)
	{
		// TODO user artist db table
		Console.WriteLine("get artists");

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

		return artistsDb;
	}

	private async Task<SpotifyUserListUpdateMain?> GetUpdateDb(string userId)
	{
		var updateDb = await _dbUpdateController.Get(userId);
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

		var updateDb = await _dbUpdateController.Get(userId);

		if (updateDb is null)
		{
			// TODO
			throw new NullReferenceException(nameof(updateDb));
		}

		// update - update times
		updateDb.Artists = update.LastUpdateMain;
		await _dbUpdateController.Update(updateDb);
	}


	private async Task SaveDb(ISet<SpotifyArtist> artists, string userId)
	{
		var userArtistsDb = await GetFollowedDb(userId);

		var newFollowedArtists = artists.Where(x => !userArtistsDb.Any(y => y.ArtistId == x.Id)).ToHashSet();
		var unfollowedArtists = userArtistsDb.Where(x => !artists.Any(y => y.Id == x.ArtistId)).ToHashSet();

		// save new followed artists
		await _dbArtistController.Save(newFollowedArtists);
		foreach (var artist in newFollowedArtists)
		{
			var artistEntity = new SpotifyUserArtistEntity(userId, artist.Id);
			await _dbTable.StoreAsync(artistEntity);
		}

		// delete not followed artists
		await Delete(unfollowedArtists);
	}

	private async Task Delete(ISet<SpotifyUserArtistEntity> userArtistsDb)
	{
		foreach (var userArtistDb in userArtistsDb)
		{
			await _dbTable.RemoveItemAsync(userArtistDb);
		}
	}
}
