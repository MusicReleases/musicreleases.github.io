using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.IdEntities;
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

	public async Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		var artistsDb = await GetEntitiesByIdsCore(ids, ct);

		var artists = artistsDb.Select(e => e.ToModel()).ToList();

		return artists.AsReadOnly();
	}
}