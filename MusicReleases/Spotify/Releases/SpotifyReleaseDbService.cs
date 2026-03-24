using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.SpotifyApi.Releases;
using System.Data;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal sealed class SpotifyReleaseDbService(IDbSpotifyService dbService)
	: IdEntityPayloadStoreService<SpotifyRelease, SpotifyReleaseEntity, SpotifyArtistReleasePayload>,
	  ISpotifyReleaseDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyReleaseEntity, string>> IdExpression => x => x.Id;

	protected override string GetEntityId(SpotifyReleaseEntity entity) => entity.Id;

	protected override string GetModelId(SpotifyRelease model) => model.Id;

	protected override SpotifyReleaseEntity ToEntity(SpotifyRelease model) => model.ToEntity();

	protected override SpotifyRelease ToModel(SpotifyReleaseEntity entity, SpotifyArtistReleasePayload payload) => entity.ToModel(payload.MainArtists, payload.FeaturedArtists);

	protected override async Task<Table<SpotifyReleaseEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Release;
	}

	public async Task<IReadOnlyCollection<SpotifyRelease>> GetByIds(IReadOnlyCollection<SpotifyArtistReleasePayload> payloads, ReleaseGroup releaseGroup, CancellationToken ct)
	{
		if (payloads.Count == 0)
		{
			return [];
		}

		var ids = payloads.Select(p => p.Id).ToHashSet();

		var entities = await FetchByIdsForGroup(ids, releaseGroup, ct);

		SetCache(entities);

		var result = new List<SpotifyRelease>(payloads.Count);

		foreach (var payload in payloads)
		{
			if (TryGetCached(payload.Id, out var entity))
			{
				result.Add(ToModel(entity, payload));
			}
		}

		return result.AsReadOnly();
	}

	private async Task<IReadOnlyCollection<SpotifyReleaseEntity>> FetchByIdsForGroup(IReadOnlyCollection<string> ids, ReleaseGroup releaseGroup, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		if (ids.Count == 0)
		{
			return [];
		}

		var table = await GetTable();

		if (releaseGroup == ReleaseGroup.Appears)
		{
			var entities = await table.BulkGet(ids);

			return entities.Where(e => e is not null).ToList().AsReadOnly();
		}

		var releaseType = EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(releaseGroup);

		var keys = ids.Select(id => (id, releaseType)).ToArray();

		var entitiesByType = await table.Where(x => x.Id, x => x.ReleaseType).AnyOf(keys).ToArray();

		return entitiesByType.Where(e => e is not null).ToList().AsReadOnly();
	}
}