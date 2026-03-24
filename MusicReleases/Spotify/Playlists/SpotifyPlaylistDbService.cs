using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Playlists;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal sealed class SpotifyPlaylistDbService(IDbSpotifyService dbService) : IdEntityPayloadStoreService<SpotifyPlaylist, SpotifyPlaylistEntity, SpotifyUserPlaylistPayload>, ISpotifyReadByPayloadService<SpotifyPlaylist, SpotifyUserPlaylistPayload>, ISpotifyWriteEntityService<SpotifyPlaylist>, ISpotifyPlaylistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyPlaylistEntity, string>> IdExpression => x => x.Id;

	protected override string GetEntityId(SpotifyPlaylistEntity entity) => entity.Id;

	protected override string GetModelId(SpotifyPlaylist model) => model.Id;

	protected override SpotifyPlaylistEntity ToEntity(SpotifyPlaylist model) => model.ToEntity();

	protected override SpotifyPlaylist ToModel(SpotifyPlaylistEntity entity, SpotifyUserPlaylistPayload payload) => entity.ToModel(payload);

	protected override async Task<Table<SpotifyPlaylistEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Playlist;
	}

	public async Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct)
	{
		await Update(playlistId, x => x.SnapshotId, newSnapshotId, ct);
	}
}