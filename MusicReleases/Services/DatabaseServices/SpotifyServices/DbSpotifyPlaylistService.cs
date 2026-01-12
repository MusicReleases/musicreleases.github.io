using DexieNET;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Mappers.Spotify;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyPlaylistService(IDbSpotifyService dbService) : IDbSpotifyPlaylistService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyList<SpotifyPlaylist>?> GetAll()
	{
		Console.WriteLine("db: get all playlists - start");

		var db = await _dbService.GetDb();
		var playlistsDb = await db.Playlist.ToArray();

		var playlists = playlistsDb.Select(e => e.ToModel()).ToArray();

		Console.WriteLine($"db: get all playlists - end");
		return playlists;
	}

	public async Task<IReadOnlyList<SpotifyPlaylist>> GetByIds(IEnumerable<string> ids)
	{
		Console.WriteLine("db: get playlists by ids - start");

		var db = await _dbService.GetDb();
		var playlistsDb = await db.Playlist.BulkGet(ids);

		var playlists = playlistsDb.Select(e => e!.ToModel()).ToArray();

		Console.WriteLine($"db: get playlists by ids - end");
		return playlists;
	}

	public async Task Save(IReadOnlyList<SpotifyPlaylist> playlists)
	{
		Console.WriteLine("db: save playlists - start");

		if (playlists.Count == 0)
		{
			return;
		}

		var playlistsDb = playlists.Select(a => a.ToEntity());

		var db = await _dbService.GetDb();
		await db.Playlist.BulkPutSafe(playlistsDb);

		Console.WriteLine("db: save playlists - end");
	}

	public async Task Add(SpotifyPlaylist playlist)
	{
		var db = await _dbService.GetDb();
		var playlistDb = playlist.ToEntity();
		await db.Playlist.Put(playlistDb);
	}


	public async Task UpdateSnapshot(string playlistId, string newSnapshotId)
	{
		var db = await _dbService.GetDb();

		await db.Playlist.Update(playlistId, p => p.SnapshotId, newSnapshotId);
	}
}
