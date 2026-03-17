using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.Artists.User;
using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistDbService(IDbSpotifyService dbService) : SpotifyIdEntityService<SpotifyArtist, SpotifyArtistEntity, SpotifyUserArtistPayload>, ISpotifyArtistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override SpotifyArtistEntity ToEntity(SpotifyArtist model) => model.ToEntity();

	protected override SpotifyArtist ToModel(SpotifyArtistEntity entity, SpotifyUserArtistPayload payload) => entity.ToModel();

	protected override async Task<Table<SpotifyArtistEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Artist;
	}

	protected override async Task<IEnumerable<SpotifyArtistEntity>> FetchByIds(string[] ids)
	{
		var table = await GetTable();

		return await table.Where(e => e.Id).AnyOf(ids).ToArray();
	}

	public async Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		// TODO DELETE

		if (ids.Count == 0)
		{
			return [];
		}

		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		var artistsDb = await db.Artist.BulkGet(ids);

		var artists = artistsDb.Select(e => e.ToModel()).ToList();

		return artists.AsReadOnly();
	}
}