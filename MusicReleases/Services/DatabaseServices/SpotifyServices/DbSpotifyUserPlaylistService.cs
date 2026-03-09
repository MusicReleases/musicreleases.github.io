using DexieNET;
using JakubKastner.MusicReleases.Mappers.Spotify;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserPlaylistService(IDbSpotifyService dbService) : IDbSpotifyUserPlaylistService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<HashSet<string>> GetUserPlaylistIds(string userId)
	{
		Console.WriteLine("db: get all user-playlists - start");
		var db = await _dbService.GetDb();

		var links = await db.UserPlaylist.Where(x => x.UserId, userId).ToArray();

		Console.WriteLine("db: get all user-playlists - end");
		return links.Select(x => x.PlaylistId).ToHashSet();
	}

	public async Task<Dictionary<string, int>> GetUserPlaylistOrder(string userId)
	{
		Console.WriteLine("db: get all user-playlists - ordered - start");
		var db = await _dbService.GetDb();
		var links = await db.UserPlaylist.Where(x => x.UserId, userId).ToArray();

		Console.WriteLine("db: get all user-playlists - ordered - start");
		return links.ToDictionary(x => x.PlaylistId, x => x.Order);
	}

	public async Task SetUserPlaylists(string userId, IEnumerable<string> apiIdsEnumerable)
	{
		Console.WriteLine("db: set user-playlists - start");

		var db = await _dbService.GetDb();

		var apiIdsList = apiIdsEnumerable.ToList();
		var currentIds = await GetUserPlaylistIds(userId);

		// remove old
		var toRemove = currentIds.Except(apiIdsList).ToList();
		if (toRemove.Count > 0)
		{
			await db.UserPlaylist.Where(x => x.UserId, userId).Filter(x => toRemove.Contains(x.PlaylistId)).Delete();
		}

		// update existing (because there are new indexes) + add new
		var linksToPut = apiIdsList.Select((pid, index) => pid.ToUserPlaylistEntity(userId, index)).ToList();

		if (linksToPut.Count > 0)
		{
			await db.UserPlaylist.BulkPutSafe(linksToPut);
		}
		Console.WriteLine("db: set user-playlists - end");
	}

	public async Task DeleteAllForUser(string userId)
	{
		var db = await _dbService.GetDb();
		await db.UserPlaylist.Where(x => x.UserId, userId).Delete();
	}


	public async Task AddUserPlaylist(string userId, string playlistId, int order)
	{
		Console.WriteLine("db: add user-playlists - start");
		var db = await _dbService.GetDb();
		var playlistDb = playlistId.ToUserPlaylistEntity(userId, order);
		await db.UserPlaylist.PutSafe(playlistDb);
		Console.WriteLine("db: add user-playlists - end");
	}
}
