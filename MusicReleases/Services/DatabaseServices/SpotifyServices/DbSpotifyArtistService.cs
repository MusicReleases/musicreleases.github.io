using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyArtistService(IDbSpotifyService dbService) : IDbSpotifyArtistService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyArtist);

	public async Task<ISet<SpotifyArtist>?> GetAll()
	{
		var sw = Stopwatch.StartNew();
		Console.WriteLine("db: get artist - start");
		// get artists from db
		Console.WriteLine("get artists");
		var artistsDb = _dbTable.GetAllAsync<SpotifyArtistEntity>();
		var artists = new HashSet<SpotifyArtist>();

		await foreach (var artistDb in artistsDb)
		{
			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name);
			artists.Add(artist);
		}

		sw.Stop();
		Console.WriteLine("db: get artist - " + sw.ElapsedMilliseconds);
		return artists;
	}

	public async Task Save(ISet<SpotifyArtist> artists)
	{
		var sw = Stopwatch.StartNew();
		Console.WriteLine("db: save artist - start");
		foreach (var artist in artists)
		{
			var artistEntity = new SpotifyArtistEntity(artist);
			await _dbTable.StoreAsync(artistEntity);
		}
		sw.Stop();
		Console.WriteLine("db: save artist - " + sw.ElapsedMilliseconds);
	}
}
