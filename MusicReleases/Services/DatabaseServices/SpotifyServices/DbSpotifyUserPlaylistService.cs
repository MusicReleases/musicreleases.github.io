using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserPlaylistService(IDbSpotifyServiceOld dbService, IDbSpotifyPlaylistService dbPlaylistService, IDbSpotifyUpdateServiceOld dbUpdateService) : IDbSpotifyUserPlaylistService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyUserPlaylist);

	private readonly IDbSpotifyPlaylistService _dbPlaylistService = dbPlaylistService;
	private readonly IDbSpotifyUpdateServiceOld _dbUpdateService = dbUpdateService;

	public async Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>?> Get(string userId)
	{
		// update
		var update = await GetUpdateDb(userId);
		if (update is null)
		{
			return null;
		}

		// artists
		var playlists = await GetSaved(userId);
		if (playlists.Count < 1)
		{
			return null;
		}

		var playlistUpdate = new SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>(playlists, update);

		return playlistUpdate;
	}

	private async Task<ISet<SpotifyPlaylist>> GetSaved(string userId)
	{
		// get followed artist ids
		var playlistsDb = await GetSavedDb(userId);
		var playlistIdsDb = playlistsDb.Select(x => x.PlaylistId);

		// get all artists (name) from db
		var playlistsDbAll = await _dbPlaylistService.GetAll();
		if (playlistsDbAll is null)
		{
			throw new NullReferenceException(nameof(playlistsDbAll));
		}

		var playlists = new HashSet<SpotifyPlaylist>();
		foreach (var playlistId in playlistIdsDb)
		{
			var playlistDb = playlistsDbAll.First(x => x.Id == playlistId);
			var playlist = new SpotifyPlaylist
			{
				Id = playlistDb.Id,
				Name = playlistDb.Name,
				CurrentUserOwned = playlistDb.CurrentUserOwned,
				Collaborative = playlistDb.Collaborative,
				SnapshotId = playlistDb.SnapshotId,
				UrlApp = playlistDb.UrlApp,
				UrlWeb = playlistDb.UrlWeb,
			};
			playlists.Add(playlist);
		}

		return playlists;
	}


	private async Task<ISet<SpotifyUserPlaylistEntity>> GetSavedDb(string userId)
	{
		// TODO user artist db table
		Console.WriteLine("db: get user playlists - start");

		// get artists from db
		var userPlaylistsDb = _dbTable.GetAllAsync<SpotifyUserPlaylistEntity>();
		var playlistsDb = new HashSet<SpotifyUserPlaylistEntity>();


		await foreach (var userPlaylistDb in userPlaylistsDb)
		{
			if (userPlaylistDb.UserId != userId)
			{
				continue;
			}
			playlistsDb.Add(userPlaylistDb);
		}
		Console.WriteLine("db: get user playlists - end");

		return playlistsDb;
	}

	private async Task<SpotifyUserListUpdatePlaylists?> GetUpdateDb(string userId)
	{
		var updateDb = await _dbUpdateService.Get(userId);
		var updateDbPlaylists = updateDb?.Playlists;
		if (!updateDbPlaylists.HasValue)
		{
			// TODO delete user artists
			return null;
		}

		var update = new SpotifyUserListUpdatePlaylists(updateDbPlaylists.Value);
		return update;
	}

	public async Task Save(string userId, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> playlists)
	{
		// TODO remove unfollowed artists and deleted releases

		if (playlists.Update is null)
		{
			// TODO
			throw new NullReferenceException(nameof(playlists.Update));
		}
		if (playlists.List is null)
		{
			// TODO
			throw new NullReferenceException(nameof(playlists.List));
		}

		// update db
		await SaveUpdateDb(userId, playlists.Update);

		// user artists db
		await SaveDb(playlists.List, userId);
	}

	private async Task SaveUpdateDb(string userId, SpotifyUserListUpdatePlaylists update)
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
		updateDb.Playlists = update.LastUpdateMain;
		await _dbUpdateService.Update(updateDb);
	}


	private async Task SaveDb(ISet<SpotifyPlaylist> playlists, string userId)
	{
		var userPlaylistsDb = await GetSavedDb(userId);

		var userPlaylistIds = userPlaylistsDb.Select(x => x.PlaylistId).ToHashSet();
		var playlistIds = playlists.Select(x => x.Id).ToHashSet();

		var newPlaylists = playlists.Where(x => !userPlaylistIds.Contains(x.Id)).ToHashSet();
		var deletedPlaylists = userPlaylistsDb.Where(x => !playlistIds.Contains(x.PlaylistId)).ToHashSet();

		// save new followed artists
		await _dbPlaylistService.Save(newPlaylists);

		Console.WriteLine("db: save user playlists - start");
		foreach (var playlist in newPlaylists)
		{
			var playlistEntity = new SpotifyUserPlaylistEntity(userId, playlist.Id);
			await _dbTable.StoreAsync(playlistEntity);
		}
		Console.WriteLine("db: save user playlists - end");

		// delete not followed artists
		await Delete(deletedPlaylists);
	}

	private async Task Delete(ISet<SpotifyUserPlaylistEntity> userPlaylistsDb)
	{
		Console.WriteLine("db: delete user playlists - start");
		foreach (var userPlaylistDb in userPlaylistsDb)
		{
			await _dbTable.RemoveItemAsync(userPlaylistDb);
		}
		Console.WriteLine("db: delete user playlists - end");
	}

	/*private async Task Delete(ISet<SpotifyUserPlaylistEntity> userPlaylistsDb)
	{
		Console.WriteLine("db: delete user playlists - start");

		await Task.WhenAll(userPlaylistsDb.Select(DeleteItem));

		Console.WriteLine("db: delete user playlists - end");
	}

	private async Task DeleteItem(SpotifyUserPlaylistEntity userPlaylistDb)
	{
		await _dbTable.RemoveItemAsync(userPlaylistDb);
	}*/

	public async Task Delete(string userId)
	{
		Console.WriteLine("db: delete user playlists by user id - start");
		var userPlaylistsDb = _dbTable.GetAllAsync<SpotifyUserPlaylistEntity>();
		await foreach (var userPlaylistDb in userPlaylistsDb)
		{
			if (userPlaylistDb.UserId != userId)
			{
				continue;
			}
			await _dbTable.RemoveItemAsync(userPlaylistDb);
		}
		Console.WriteLine("db: delete user playlists by user id - end");
	}
}
