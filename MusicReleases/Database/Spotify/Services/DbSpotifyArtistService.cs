using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyArtistService(IDbSpotifyService dbService) : IDbSpotifyArtistService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyList<SpotifyArtist>?> GetAll(CancellationToken ct)
	{
		Console.WriteLine("db: get all artists - start");

		var db = await _dbService.GetDb();

		ct.ThrowIfCancellationRequested();
		var artistsDb = await db.Artist.ToArray();

		var artists = artistsDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get all artists - end");
		return artists;
	}

	public async Task<IReadOnlyList<SpotifyArtist>> GetByIds(IEnumerable<string> ids, CancellationToken ct)
	{
		Console.WriteLine("db: get artists by ids - start");

		var db = await _dbService.GetDb();

		ct.ThrowIfCancellationRequested();
		var artistsDb = await db.Artist.BulkGet(ids);

		var artists = artistsDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get artists by ids - end");
		return artists;
	}

	public async Task Save(IReadOnlyList<SpotifyArtist> artists, CancellationToken ct)
	{
		Console.WriteLine("db: save artists - start");

		if (artists.Count == 0)
		{
			return;
		}

		var artistsDb = artists.Select(a => a.ToEntity());

		var db = await _dbService.GetDb();

		ct.ThrowIfCancellationRequested();
		await db.Artist.BulkPutSafe(artistsDb);

		Console.WriteLine("db: save artists - end");
	}
}