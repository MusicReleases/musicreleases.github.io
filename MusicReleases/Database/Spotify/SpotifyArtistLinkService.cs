using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal abstract class SpotifyArtistLinkService<TArtistLinkEntity>
	: SpotifyLinkEntityService<TArtistLinkEntity, string, string>, ISpotifyArtistLinkService<TArtistLinkEntity> where TArtistLinkEntity : ISpotifyDb, ISpotifyArtistLinkEntity
{
	private readonly Dictionary<string, SpotifyArtistGroupPayload> _cache = [];

	protected abstract Task<IEnumerable<TArtistLinkEntity>> FetchByKeys2(IEnumerable<string> releaseIds);
	protected abstract Task<IEnumerable<SpotifyArtistReleaseEntity>> FetchByArtistsAndGroup(IEnumerable<string> artistIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType);
	protected abstract Task<IEnumerable<SpotifyArtistReleaseEntity>> FetchByReleasesAndGroup(IEnumerable<string> releaseIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType);

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

			_cache[group.Key] = new(group.Key, mainArtistIds, featuredArtistIds); ;



			/*if (!_cache.TryGetValue(group.Key, out var cached))
			{
				continue;
			}
			var releaseIds = group.Select(x => x.ReleaseId).ToHashSet();

			foreach (var releaseId in releaseIds)
			{
				var payload = new SpotifyArtistGroupPayload(
					releaseId,
					group.Where(x => x.ReleaseId == releaseId && x.Role == ArtistReleaseRole.Main)
						 .Select(x => x.ArtistId).ToHashSet(),
					group.Where(x => x.ReleaseId == releaseId && x.Role == ArtistReleaseRole.Featured)
						 .Select(x => x.ArtistId).ToHashSet()
				);

				cached.Add(payload);
			}*/
		}
	}


	public async Task<IReadOnlyCollection<SpotifyArtistGroupPayload>> GetArtistsByArtistIds(IReadOnlyCollection<string> artistIds, ReleaseGroup releaseGroup, CancellationToken ct)
	{
		if (artistIds.Count == 0)
		{
			return [];
		}

		var artistRole = EnumReleaseTypeExtensions.MapReleaseRoleFromGroup(releaseGroup);

		ReleaseType? releaseType
			= releaseGroup == ReleaseGroup.Appears
			? null
			: EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(releaseGroup);

		var releases = await GetReleasesByArtistIds(artistIds, artistRole, releaseType, ct);
		var releaseIds = releases.Select(x => x.ReleaseId).ToHashSet();


		var missingReleaseIds = releaseIds.Where(id => !_cache.ContainsKey(id)).ToList();

		if (missingReleaseIds.Count > 0)
		{
			var artists = await GetArtistsByReleasesIds(releaseIds, releaseType, ct);

			var payloads = artists
				.GroupBy(x => x.ReleaseId)
				.Select(g => new SpotifyArtistGroupPayload(
					g.Key,
					g.Where(x => x.Role == ArtistReleaseRole.Main).Select(x => x.ArtistId).ToHashSet(),
					g.Where(x => x.Role == ArtistReleaseRole.Featured).Select(x => x.ArtistId).ToHashSet()
				))
				.ToList();


			foreach (var payload in payloads)
			{
				_cache[payload.ReleaseId] = payload;
			}


		}
		return releaseIds.Select(id => _cache[id]).ToList().AsReadOnly();

		/*if (artistIds.Count == 0)
		{
			return [];
		}

		var missingArtistIds = artistIds.Where(id => !_cache.ContainsKey(id)).ToList();

		if (missingArtistIds.Count > 0)
		{
			var table = await GetTable();

			var releasesByArtists = await FetchByArtistsAndGroup(missingArtistIds, releaseGroup);

			var releaseIds = releasesByArtists.Select(x => x.ReleaseId).ToHashSet();

			var artistsByReleases = await FetchByReleasesAndGroup(releaseIds, releaseGroup);

			var payloadsByRelease = artistsByReleases
				.GroupBy(x => x.ReleaseId)
				.ToDictionary(
					g => g.Key,
					g => new SpotifyArtistGroupPayload(
						g.Key,
						[.. g.Where(x => x.Role == ArtistReleaseRole.Main).Select(x => x.ArtistId)],
						[.. g.Where(x => x.Role == ArtistReleaseRole.Featured).Select(x => x.ArtistId)]
					));

			foreach (var artistId in missingArtistIds)
			{
				_cache[artistId] = releasesByArtists
					.Where(x => x.ArtistId == artistId)
					.Select(x => x.ReleaseId)
					.Where(payloadsByRelease.ContainsKey)
					.Select(releaseId => payloadsByRelease[releaseId])
					.ToList();
			}

		}
		return artistIds.Where(_cache.ContainsKey).SelectMany(id => _cache[id]).DistinctBy(p => p.ReleaseId).ToList();*/
	}

	private async Task<IEnumerable<SpotifyArtistReleaseEntity>> GetReleasesByArtistIds(IEnumerable<string> artistIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType, CancellationToken ct)
	{
		var releasesByArtists = await FetchByArtistsAndGroup(artistIds, artistRole, releaseType);

		return releasesByArtists;
	}

	private async Task<IEnumerable<SpotifyArtistReleaseEntity>> GetArtistsByReleasesIds(IEnumerable<string> releaseIds, ReleaseType? releaseType, CancellationToken ct)
	{
		var releasesByArtists = await FetchByReleasesAndGroup(releaseIds, null, releaseType);

		return releasesByArtists;
	}

	public async Task<Dictionary<string, IReadOnlyCollection<string>>> GetReleasesByArtistIds(IReadOnlyCollection<string> artistIds, ReleaseType releaseType, CancellationToken ct)
	{
		var linksByArtists = await FetchByKeys1(artistIds);

		var releases = linksByArtists
			.GroupBy(x => x.ReleaseId)
			.ToDictionary(
				g => g.Key,
				g => new SortedSet<string>(g.Select(x => x.ArtistId)) as IReadOnlyCollection<string>
			);

		return releases;
	}

	/*public async Task<SpotifyArtistGroupPayload> GetArtistsByReleaseIds(Dictionary<string, IReadOnlyCollection<string>> releaseArtistIds, CancellationToken ct)
	{
		var releaseIds = releaseArtistIds.Select(x => x.Key);

		var linksByArtists = await FetchByKeys2(releaseIds);



	}*/

	/*public async Task<IReadOnlyCollection<SpotifyArtistGroupPayload>> GetArtistsByArtistIds(IReadOnlyCollection<string> artistIds, CancellationToken ct)
	{
		if (artistIds.Count == 0)
		{
			return [];
		}

		var missingArtistIds = artistIds.Where(id => !_cache.ContainsKey(id)).ToList();

		if (missingArtistIds.Count > 0)
		{
			var table = await GetTable();

			var linksByArtists = await FetchByKeys1(missingArtistIds);

			var releaseIds = linksByArtists.Select(x => x.ReleaseId).ToHashSet();

			var linksByReleases = await FetchByKeys2(releaseIds);

			var payloadsByRelease = linksByReleases
				.GroupBy(x => x.ReleaseId)
				.ToDictionary(
					g => g.Key,
					g => new SpotifyArtistGroupPayload(
						g.Key,
						[.. g.Where(x => x.Role == ArtistReleaseRole.Main).Select(x => x.ArtistId)],
						[.. g.Where(x => x.Role == ArtistReleaseRole.Featured).Select(x => x.ArtistId)]
					));

			foreach (var artistId in missingArtistIds)
			{
				_cache[artistId] = linksByArtists
					.Where(x => x.ArtistId == artistId)
					.Select(x => x.ReleaseId)
					.Where(payloadsByRelease.ContainsKey)
					.Select(releaseId => payloadsByRelease[releaseId])
					.ToList();
			}

		}
		return artistIds.Where(_cache.ContainsKey).SelectMany(id => _cache[id]).DistinctBy(p => p.ReleaseId).ToList();
	}*/
}