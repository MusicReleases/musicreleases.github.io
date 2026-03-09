using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyTrackService(IDbSpotifyService dbService) : IDbSpotifyTrackService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyList<SpotifyTrack>?> GetAll()
	{
		Console.WriteLine("db: get all tracks - start");

		var db = await _dbService.GetDb();
		var tracksDb = await db.Track.ToArray();

		var tracks = tracksDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get all tracks - end");
		return tracks;
	}

	public async Task<IReadOnlyList<SpotifyTrack>> GetByReleaseId(string releaseId)
	{
		Console.WriteLine("db: get tracks by release id - start");

		var db = await _dbService.GetDb();
		var tracksDb = await db.Track.Where(x => x.ReleaseId, releaseId).ToArray();

		var tracks = tracksDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get tracks by release id - end");
		return tracks;
	}

	public async Task<IReadOnlyList<SpotifyTrack>> GetByIds(IEnumerable<string> ids)
	{
		Console.WriteLine("db: get tracks by ids - start");

		var db = await _dbService.GetDb();
		var tracksDb = await db.Track.BulkGet(ids);

		var tracks = tracksDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get tracks by ids - end");
		return tracks;
	}

	public async Task Save(IReadOnlyList<SpotifyTrack> tracks)
	{
		Console.WriteLine("db: save tracks - start");

		if (tracks.Count == 0)
		{
			return;
		}

		var tracksDb = tracks.Select(a => a.ToEntity());

		var db = await _dbService.GetDb();
		await db.Track.BulkPutSafe(tracksDb);

		Console.WriteLine("db: save tracks - end");
	}
}