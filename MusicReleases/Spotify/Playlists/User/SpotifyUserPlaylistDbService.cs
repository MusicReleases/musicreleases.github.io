using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services.Links;
using JakubKastner.SpotifyApi.Playlists;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal sealed class SpotifyUserPlaylistDbService(IDbSpotifyService dbService) : SpotifyUserLinkEntityService<SpotifyUserPlaylistEntity, SpotifyUserPlaylistPayload>, ISpotifyUserPlaylistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyUserPlaylistEntity, string>> Key1Expression => x => x.UserId;

	protected override Expression<Func<SpotifyUserPlaylistEntity, string>> Key2Expression => x => x.PlaylistId;

	protected override SpotifyUserPlaylistEntity ToEntityFromKey1(SpotifyUserPlaylistPayload payload, string userId) => payload.ToEntity(userId);

	protected override SpotifyUserPlaylistPayload ToPayload1(SpotifyUserPlaylistEntity entity) => entity.ToPayload();


	protected override async Task<Table<SpotifyUserPlaylistEntity, (string, string)>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.UserPlaylist;
	}

	public async Task AddNew(SpotifyPlaylist playlist, string userId, CancellationToken ct)
	{
		var payload = playlist.ToPayload();
		await Save(userId, payload, ct);
	}
}
