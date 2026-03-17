using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal sealed class SpotifyPlaylistDbService(IDbSpotifyService dbService) : SpotifyEntityService<SpotifyPlaylist, SpotifyPlaylistEntity, SpotifyUserPlaylistPayload>, ISpotifyPlaylistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override SpotifyPlaylistEntity ToEntity(SpotifyPlaylist model) => model.ToEntity();

	protected override SpotifyPlaylist ToModel(SpotifyPlaylistEntity entity, SpotifyUserPlaylistPayload payload) => entity.ToModel(payload);

	protected override async Task<Table<SpotifyPlaylistEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Playlist;
	}

	protected override async Task<IEnumerable<SpotifyPlaylistEntity>> FetchByIds(string[] ids)
	{
		var table = await GetTable();

		return await table.Where(e => e.Id).AnyOf(ids).ToArray();
	}

	public async Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct)
	{
		var table = await GetTable();
		ct.ThrowIfCancellationRequested();

		await table.Update(playlistId, p => p.SnapshotId, newSnapshotId);

		_cache[playlistId].SnapshotId = newSnapshotId;
	}
}