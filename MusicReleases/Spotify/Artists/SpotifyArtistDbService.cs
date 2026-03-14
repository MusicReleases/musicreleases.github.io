using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistDbService(IDbSpotifyService dbService) : ISpotifyArtistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<IReadOnlyCollection<SpotifyArtist>> GetAll(CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		var artistsDb = await db.Artist.ToArray();

		var artists = artistsDb.Select(e => e.ToModel()).ToList();

		return artists.AsReadOnly();
	}

	public async Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
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

	public async Task Save(IReadOnlyCollection<SpotifyArtist> artists, CancellationToken ct)
	{
		if (artists.Count == 0)
		{
			return;
		}

		var artistsDb = artists.Select(a => a.ToEntity());

		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		await db.Artist.BulkPutSafe(artistsDb);
	}
}