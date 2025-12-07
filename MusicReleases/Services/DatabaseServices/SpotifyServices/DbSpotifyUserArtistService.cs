using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserArtistService(IDbSpotifyService dbService) : IDbSpotifyUserArtistService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyCollection<string>> GetFollowedIds(string userId)
	{
		var db = await _dbService.GetDb();

		/*var keys = await db.UserArtist.Where(x => x.UserId, userId).PrimaryKeys();
		var ids = keys.Select(k => k.ArtistId).ToHashSet();*/

		var links = await db.UserArtist.Where(x => x.UserId, userId).ToArray();
		var ids = links.Select(x => x.ArtistId).ToHashSet();

		return ids;
	}

	public async Task SetFollowed(string userId, IEnumerable<string> apiIds)
	{
		var incomingIds = apiIds.ToHashSet();

		var currentIds = await GetFollowedIds(userId);

		var toAdd = incomingIds.Except(currentIds).ToList();
		var toRemove = currentIds.Except(incomingIds).ToList();

		if (toAdd.Count > 0 || toRemove.Count > 0)
		{
			await ApplyChanges(userId, toAdd, toRemove);
		}
	}

	private async Task ApplyChanges(string userId, List<string> toAdd, List<string> toRemove)
	{
		var db = await _dbService.GetDb();

		if (toAdd.Count > 0)
		{
			//var newLinks = toAdd.Select(aid => new SpotifyUserArtistEntity((userId, aid), userId));

			var newLinks = toAdd.Where(aid => !string.IsNullOrEmpty(aid)).Select(aid => new SpotifyUserArtistEntity(userId, aid)).ToList();
			await db.UserArtist.BulkPut(newLinks);
		}

		if (toRemove.Count > 0)
		{
			/*var keysToDelete = toRemove.Select(aid => (userId, aid));
			await db.UserArtist.BulkDelete(keysToDelete);*/

			var keysToDelete = toRemove.Select(aid => (userId, aid)).ToArray();

			// Teď už BulkDelete pozná, že dostává správné klíče
			await db.UserArtist.BulkDelete(keysToDelete);
		}
	}
}