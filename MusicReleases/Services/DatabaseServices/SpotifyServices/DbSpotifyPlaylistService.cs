using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyPlaylistService(IDbSpotifyService dbService) : IDbSpotifyPlaylistService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyPlaylist);

	public async Task<ISet<SpotifyPlaylist>?> GetAll()
	{
		// get playlists from db
		Console.WriteLine("db: get playlists - start");
		var playlistsDb = _dbTable.GetAllAsync<SpotifyPlaylistEntity>();
		var playlists = new HashSet<SpotifyPlaylist>();

		await foreach (var playlistDb in playlistsDb)
		{
			var playlist = new SpotifyPlaylist
			{
				Id = playlistDb.Id,
				Name = playlistDb.Name,
				CurrentUserOwned = playlistDb.CurrentUserOwned,
				Collaborative = playlistDb.Collaborative,
			};
			playlists.Add(playlist);
		}

		Console.WriteLine("db: get playlists - end");
		return playlists;
	}

	public async Task Save(ISet<SpotifyPlaylist> playlists)
	{
		Console.WriteLine("db: save playlists - start");
		foreach (var playlist in playlists)
		{
			var artistEntity = new SpotifyPlaylistEntity(playlist);
			await _dbTable.StoreAsync(artistEntity);
		}
		Console.WriteLine("db: save playlists - start");
	}
}
