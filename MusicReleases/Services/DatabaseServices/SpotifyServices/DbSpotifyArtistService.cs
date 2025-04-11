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
		Console.WriteLine("get artists");
		var artistsDb = _dbTable.GetAllAsync<SpotifyArtistEntity>();
		var artists = new HashSet<SpotifyArtist>();

		await foreach (var artistDb in artistsDb)
		{
			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name);
			artists.Add(artist);
		}

		return artists;
	}

	public async Task Save(ISet<SpotifyArtist> artists)
	{
		var artistsDb = new HashSet<SpotifyArtistEntity>();
		foreach (var artist in artists)
		{
			var artistEntity = new SpotifyArtistEntity(artist);

			artistsDb.Add(artistEntity);
		}
		Console.WriteLine("save artists");
		await _dbTable.StoreItemsAsync(artistsDb);
	}
}
