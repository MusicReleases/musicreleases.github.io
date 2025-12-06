using DexieNET;
using JakubKastner.MusicReleases.Mappers.Spotify;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyArtistService(IDbSpotifyService dbService) : IDbSpotifyArtistService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyList<SpotifyArtist>?> GetAll()
	{
		Console.WriteLine("db: get all artists - start");

		var db = await _dbService.GetDb();
		var entities = await db.Artists.ToArray();

		var artists = entities.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get all artists - end");
		return artists;
	}

	public async Task Save(IReadOnlyList<SpotifyArtist> artists)
	{
		Console.WriteLine("db: save artists - start");

		var entities = artists.Select(a => a.ToEntity());

		var db = await _dbService.GetDb();
		await db.Artists.BulkPut(entities);

		Console.WriteLine("db: save artists - end");
	}
}