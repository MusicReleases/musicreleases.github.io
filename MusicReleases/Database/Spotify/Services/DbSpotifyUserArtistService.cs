using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyUserArtistService(IDbSpotifyService dbService) : IDbSpotifyUserArtistService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyCollection<string>> GetFollowedIds(string userId, CancellationToken ct)
	{
		Console.WriteLine("db: get all user-artists - start");
		var db = await _dbService.GetDb();

		var links = await db.UserArtist.Where(x => x.UserId, userId).ToArray();

		ct.ThrowIfCancellationRequested();
		var artistIds = links.Select(x => x.ArtistId).ToHashSet();

		Console.WriteLine("db: get all user-artists - end");
		return artistIds;
	}

	public async Task SetFollowed(string userId, IEnumerable<string> apiIds, CancellationToken ct)
	{
		Console.WriteLine("db: set user-artists - start");
		var incomingIds = apiIds.ToHashSet();

		var currentIds = await GetFollowedIds(userId, ct);
		ct.ThrowIfCancellationRequested();

		var toAdd = incomingIds.Except(currentIds).ToList();
		var toRemove = currentIds.Except(incomingIds).ToList();

		if (toAdd.Count > 0 || toRemove.Count > 0)
		{
			await ApplyChanges(userId, toAdd, toRemove, ct);
		}
		Console.WriteLine("db: set user-artists - end");
	}

	private async Task ApplyChanges(string userId, List<string> toAdd, List<string> toRemove, CancellationToken ct)
	{
		var db = await _dbService.GetDb();

		if (toAdd.Count > 0)
		{
			var newLinks = toAdd.Where(artistId => !string.IsNullOrEmpty(artistId)).Select(artistId => artistId.ToSpotifyUserArtistEntity(userId)).ToList();

			ct.ThrowIfCancellationRequested();
			await db.UserArtist.BulkPutSafe(newLinks);
		}

		if (toRemove.Count > 0)
		{
			ct.ThrowIfCancellationRequested();
			await db.UserArtist.Where(x => x.UserId, userId).Filter(x => toRemove.Contains(x.ArtistId)).Delete();
		}
	}
}