using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyPlaylistServiceOld(IDbSpotifyServiceOld dbService) : IDbSpotifyPlaylistServiceOld
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
				SnapshotId = playlistDb.SnapshotId,
				UrlApp = playlistDb.UrlApp,
				UrlWeb = playlistDb.UrlWeb,

				OwnerId = "",
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
		Console.WriteLine("db: save playlists - end");
	}
}
