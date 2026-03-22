using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.IdEntities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal sealed class SpotifyPlaylistDbService(IDbSpotifyService dbService) : SpotifyIdEntityService<SpotifyPlaylist, SpotifyPlaylistEntity, SpotifyUserPlaylistPayload>, ISpotifyPlaylistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override SpotifyPlaylistEntity ToEntity(SpotifyPlaylist model) => model.ToEntity();

	protected override SpotifyPlaylist ToModel(SpotifyPlaylistEntity entity, SpotifyUserPlaylistPayload payload) => entity.ToModel(payload);

	protected override async Task<Table<SpotifyPlaylistEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Playlist;
	}

	public async Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct)
	{
		await Update(playlistId, p => p.SnapshotId, newSnapshotId, ct);
	}
}