using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyPlaylistService(IDbSpotifyService dbService) : IDbSpotifyPlaylistService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyList<SpotifyPlaylist>?> GetAll(CancellationToken ct)
	{
		Console.WriteLine("db: get all playlists - start");

		var db = await _dbService.GetDb();

		ct.ThrowIfCancellationRequested();
		var playlistsDb = await db.Playlist.ToArray();
		ct.ThrowIfCancellationRequested();

		var playlists = playlistsDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get all playlists - end");
		return playlists;
	}

	public async Task<IReadOnlyList<SpotifyPlaylist>> GetByIds(IEnumerable<string> ids, CancellationToken ct)
	{
		Console.WriteLine("db: get playlists by ids - start");

		var db = await _dbService.GetDb();

		ct.ThrowIfCancellationRequested();
		var playlistsDb = await db.Playlist.BulkGet(ids);
		ct.ThrowIfCancellationRequested();

		var playlists = playlistsDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get playlists by ids - end");
		return playlists;
	}

	public async Task Save(IReadOnlyList<SpotifyPlaylist> playlists, CancellationToken ct)
	{
		Console.WriteLine("db: save playlists - start");

		if (playlists.Count == 0)
		{
			return;
		}

		var playlistsDb = playlists.Select(a => a.ToEntity());

		var db = await _dbService.GetDb();

		ct.ThrowIfCancellationRequested();
		await db.Playlist.BulkPutSafe(playlistsDb);

		Console.WriteLine("db: save playlists - end");
	}

	public async Task Add(SpotifyPlaylist playlist, CancellationToken ct)
	{
		Console.WriteLine("db: add playlist - start");

		var db = await _dbService.GetDb();
		var playlistDb = playlist.ToEntity();

		ct.ThrowIfCancellationRequested();
		await db.Playlist.PutSafe(playlistDb);

		Console.WriteLine("db: add playlist - end");
	}


	public async Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct)
	{
		Console.WriteLine("db: update playlist snapshot - start");
		var db = await _dbService.GetDb();

		ct.ThrowIfCancellationRequested();

		await db.Playlist.Update(playlistId, p => p.SnapshotId, newSnapshotId);
		Console.WriteLine("db: update playlist snapshot - end");
	}
}
