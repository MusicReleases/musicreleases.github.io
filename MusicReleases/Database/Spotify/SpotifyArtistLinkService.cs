using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal abstract class SpotifyArtistLinkService<TArtistLinkEntity>
	: SpotifyLinkEntityService<TArtistLinkEntity, string, string>, ISpotifyArtistLinkEntityService<TArtistLinkEntity> where TArtistLinkEntity : ISpotifyDb, ISpotifyArtistLinkEntity
{
	protected readonly Dictionary<string, SpotifyArtistGroupPayload> _cache = [];

	protected override async Task<IEnumerable<TArtistLinkEntity>> FetchByKeys1(IEnumerable<string> artistIds)
	{
		var table = await GetTable();

		var linksByArtists = await table.Where(x => x.ArtistId).AnyOf([.. artistIds]).ToArray();

		return linksByArtists;
	}

	public async Task<IEnumerable<TArtistLinkEntity>> FetchByKeys2(IEnumerable<string> releaseIds)
	{
		var table = await GetTable();

		var linksByReleases = await table.Where(x => x.ReleaseId).AnyOf([.. releaseIds]).ToArray();

		return linksByReleases;
	}

	public async Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct)
	{
		await SaveEntities(entities, ct);

		foreach (var group in entities.GroupBy(x => x.ReleaseId))
		{
			if (_cache.TryGetValue(group.Key, out var payload))
			{
				// saved
				continue;
			}

			HashSet<string> mainArtistIds = [];
			HashSet<string> featuredArtistIds = [];

			foreach (var entity in group)
			{
				if (entity.Role == ArtistReleaseRole.Main)
				{
					mainArtistIds.Add(entity.ArtistId);
				}
				else if (entity.Role == ArtistReleaseRole.Featured)
				{
					featuredArtistIds.Add(entity.ArtistId);
				}
			}

			_cache[group.Key] = new(group.Key, mainArtistIds, featuredArtistIds);
		}
	}
}