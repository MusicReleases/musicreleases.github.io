using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyArtistService(IDbSpotifyService dbService) : IDbSpotifyArtistService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyArtist);

	public async Task<ISet<SpotifyArtist>?> GetAll()
	{
		// get artists from db
		Console.WriteLine("db: get all artists - start");
		var artistsDb = _dbTable.GetAllAsync<SpotifyArtistEntity>();
		var artists = new HashSet<SpotifyArtist>();

		await foreach (SpotifyArtistEntity artistDb in artistsDb)
		{
			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name, artistDb.UrlApp, artistDb.UrlWeb);
			artists.Add(artist);
		}

		Console.WriteLine("db: get all artists - end");
		return artists;
	}

	public async Task Save(ISet<SpotifyArtist> artists)
	{
		Console.WriteLine("db: save artists - start");
		foreach (var artist in artists)
		{
			var artistEntity = new SpotifyArtistEntity(artist);
			await _dbTable.StoreAsync(artistEntity);
		}
		Console.WriteLine("db: save artists - end");
	}
}
