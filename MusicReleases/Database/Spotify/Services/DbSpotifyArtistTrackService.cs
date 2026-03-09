using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyArtistTrackService(IDbSpotifyService dbService) : IDbSpotifyArtistTrackService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<HashSet<string>> GetArtistTrackIds(string artistId)
	{
		var db = await _dbService.GetDb();

		var links = await db.ArtistTrack.Where(x => x.ArtistId, artistId).ToArray();

		return links.Select(x => x.TrackId).ToHashSet();
	}

	public async Task SetArtistTracks(string artistId, IEnumerable<string> trackApiIdsEnumerable)
	{

		var db = await _dbService.GetDb();

		var apiIds = trackApiIdsEnumerable.ToList();
		var currentIds = await GetArtistTrackIds(artistId);

		// remove old
		var toRemove = currentIds.Except(apiIds).ToList();
		if (toRemove.Count > 0)
		{
			await db.ArtistTrack.Where(x => x.ArtistId, artistId).Filter(x => toRemove.Contains(x.TrackId)).Delete();
		}

		// add new
		var toAdd = apiIds.Except(currentIds).Select(rid => rid.ToArtistTrackEntity(artistId)).ToList();
		if (toAdd.Count > 0)
		{
			await db.ArtistTrack.BulkPutSafe(toAdd);
		}
	}

	public async Task DeleteAllForArtist(string artistId)
	{
		var db = await _dbService.GetDb();
		await db.ArtistTrack.Where(x => x.ArtistId, artistId).Delete();
	}


	public async Task AddArtistTrack(string artistId, string releaseId)
	{
		var db = await _dbService.GetDb();
		var playlistDb = releaseId.ToArtistTrackEntity(artistId);
		await db.ArtistTrack.PutSafe(playlistDb);
	}
}
