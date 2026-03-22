using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.IdEntities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.SpotifyApi.Releases;
using System.Data;

namespace JakubKastner.MusicReleases.Spotify.Releases;


internal sealed class SpotifyReleaseDbService(IDbSpotifyService dbService) : SpotifyIdEntityService<SpotifyRelease, SpotifyReleaseEntity, SpotifyArtistReleasePayload>, ISpotifyReleaseDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	private ReleaseGroup _currentReleaseGroup;

	protected override SpotifyReleaseEntity ToEntity(SpotifyRelease model) => model.ToEntity();

	protected override SpotifyRelease ToModel(SpotifyReleaseEntity entity, SpotifyArtistReleasePayload payload) => entity.ToModel(payload.MainArtists, payload.FeaturedArtists);

	protected override async Task<Table<SpotifyReleaseEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Release;
	}

	public Task<IReadOnlyCollection<SpotifyRelease>> GetByIds(IReadOnlyCollection<SpotifyArtistReleasePayload> payloads, ReleaseGroup releaseGroup, CancellationToken ct)
	{
		_currentReleaseGroup = releaseGroup;
		return GetByIds(payloads, ct);
	}

	protected override async Task<IReadOnlyCollection<SpotifyReleaseEntity>> FetchByIds(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		if (_currentReleaseGroup == ReleaseGroup.Appears)
		{
			var entities = await table.BulkGet(ids);
			return entities.ToList().AsReadOnly();
		}

		var releaseType = EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(_currentReleaseGroup);

		var keys = ids.Select(id => (id, releaseType)).ToArray();

		var itemsDb = await table.Where(x => x.Id, x => x.ReleaseType).AnyOf(keys).ToArray();

		return itemsDb.ToList().AsReadOnly();
	}

}