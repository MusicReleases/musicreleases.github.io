using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal sealed class SpotifyUserPlaylistDbService(IDbSpotifyService dbService) : SpotifyUserLinkEntityService<SpotifyUserPlaylistEntity, SpotifyUserPlaylistPayload>, ISpotifyUserPlaylistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override SpotifyUserPlaylistPayload ToPayload(SpotifyUserPlaylistEntity entity) => entity.ToPayload();

	protected override SpotifyUserPlaylistEntity ToEntity(SpotifyUserPlaylistPayload payload, string userId) => payload.ToEntity(userId);

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

	public async Task AddNew(SpotifyPlaylist playlist, string userId, CancellationToken ct)
	{
		var payload = new SpotifyUserPlaylistPayload(playlist.Id, playlist.Order);
		await Save(payload, userId, ct);
	}
}
