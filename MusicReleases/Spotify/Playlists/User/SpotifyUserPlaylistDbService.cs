using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal sealed class SpotifyUserPlaylistDbService(IDbSpotifyService dbService) : SpotifyUserRelationService<SpotifyUserPlaylistEntity, SpotifyUserPlaylistPayload>, ISpotifyUserPlaylistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override SpotifyUserPlaylistPayload ToPayload(SpotifyUserPlaylistEntity entity) => entity.ToPayload();

	protected override SpotifyUserPlaylistEntity CreateEntity(SpotifyUserPlaylistPayload payload, string userId) => payload.ToEntity(userId);

	protected override async Task<Table<SpotifyUserPlaylistEntity, (string, string)>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.UserPlaylist;
	}

	protected override async Task<IEnumerable<SpotifyUserPlaylistEntity>> FetchByUserId(string userId)
	{
		var table = await GetTable();
		return await table.Where(x => x.UserId, userId).ToArray();
	}

	public async Task DeleteAllForUser(string userId)
	{
		var db = await _dbService.GetDb();
		await db.UserPlaylist.Where(x => x.UserId, userId).Delete();
	}

	public async Task AddNew(string playlistId, string userId, CancellationToken ct)
	{
		var payload = new SpotifyUserPlaylistPayload(playlistId, 0);
		await Save(payload, userId, ct);
	}
}
