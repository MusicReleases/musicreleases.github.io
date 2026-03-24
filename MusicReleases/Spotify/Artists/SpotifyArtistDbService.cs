using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.Artists.User;
using JakubKastner.SpotifyApi.Artists;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistDbService(IDbSpotifyService dbService) : IdEntityStoreService<SpotifyArtist, SpotifyArtistEntity>, ISpotifyArtistDbService, IReadByPayloadService<SpotifyArtist, SpotifyUserArtistPayload>, IWriteEntityService<SpotifyArtist>
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyArtistEntity, string>> IdExpression
	{
		get
		{
			return x => x.Id;
		}
	}

	protected override string GetEntityId(SpotifyArtistEntity entity)
	{
		return entity.Id;
	}

	protected override string GetModelId(SpotifyArtist model)
	{
		return model.Id;
	}

	protected override SpotifyArtistEntity ToEntity(SpotifyArtist model)
	{
		return model.ToEntity();
	}

	protected override SpotifyArtist ToModel(SpotifyArtistEntity entity)
	{
		return entity.ToModel();
	}

	protected override async Task<Table<SpotifyArtistEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Artist;
	}

	public async Task<SpotifyArtist?> GetById(SpotifyUserArtistPayload payload, CancellationToken ct)
	{
		return await GetById(payload.Id, ct);
	}

	public async Task<IReadOnlyCollection<SpotifyArtist>> GetByIds(IReadOnlyCollection<SpotifyUserArtistPayload> payloads, CancellationToken ct)
	{
		if (payloads.Count == 0)
		{
			return [];
		}

		var ids = payloads
			.Select(p => p.Id)
			.ToHashSet();

		await GetEntitiesCore(ids, true, ct);

		var result = new List<SpotifyArtist>(payloads.Count);

		foreach (var payload in payloads)
		{
			if (TryGetCached(payload.Id, out var entity))
			{
				result.Add(ToModel(entity));
			}
		}

		return result.AsReadOnly();
	}
}