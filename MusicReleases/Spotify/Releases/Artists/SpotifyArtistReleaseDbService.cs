using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Links;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases.Artists;

internal sealed class SpotifyArtistReleaseDbService(IDbSpotifyService dbService) : SpotifyArtistLinkService<SpotifyArtistReleaseEntity>, ISpotifyArtistReleaseDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override async Task<Table<SpotifyArtistReleaseEntity, (string, string)>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.ArtistRelease;
	}

	public async Task<IReadOnlyCollection<SpotifyArtistGroupByReleasePayload>> GetArtistsByArtistIds(IReadOnlyCollection<string> artistIds, ReleaseGroup releaseGroup, CancellationToken ct)
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

		var releases = await GetByArtists(artistIds, artistRole, releaseType, ct);
		var releaseIds = releases.Select(x => x.ReleaseId).ToHashSet();

		var missingReleaseIds = releaseIds.Where(id => !_cache.ContainsKey(id)).ToList();

		if (missingReleaseIds.Count > 0)
		{
			var artists = await GetByReleases(releaseIds, null, releaseType, ct);

			var payloads = artists
				.GroupBy(x => x.ReleaseId)
				.Select(g => new SpotifyArtistGroupByReleasePayload
				(
					g.Key,
					g.Where(x => x.Role == ArtistReleaseRole.Main).Select(x => x.ArtistId).ToHashSet(),
					g.Where(x => x.Role == ArtistReleaseRole.Featured).Select(x => x.ArtistId).ToHashSet()
				))
				.ToList();


			foreach (var payload in payloads)
			{
				_cache[payload.Id] = payload;
			}
		}

		return releaseIds.Select(id => _cache[id]).ToList().AsReadOnly();
	}

	private async Task<IEnumerable<SpotifyArtistReleaseEntity>> GetByArtists(IEnumerable<string> artistIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType, CancellationToken ct)
	{
		var table = await GetTable();

		IEnumerable<SpotifyArtistReleaseEntity>? linksByReleases;
		ct.ThrowIfCancellationRequested();

		if (artistRole.HasValue && releaseType.HasValue)
		{
			var links = artistIds.Select(id => (id, artistRole.Value, releaseType.Value));
			linksByReleases = await table.Where(x => x.ArtistId, x => x.Role, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}
		else if (artistRole.HasValue)
		{
			var links = artistIds.Select(id => (id, artistRole.Value));
			linksByReleases = await table.Where(x => x.ArtistId, x => x.Role).AnyOf([.. links]).ToArray();
		}
		else if (releaseType.HasValue)
		{
			var links = artistIds.Select(id => (id, releaseType.Value));
			linksByReleases = await table.Where(x => x.ArtistId, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}
		else
		{
			var links = artistIds;
			linksByReleases = await table.Where(x => x.ArtistId).AnyOf([.. links]).ToArray();
		}

		return linksByReleases;
	}

	private async Task<IEnumerable<SpotifyArtistReleaseEntity>> GetByReleases(IEnumerable<string> releaseIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType, CancellationToken ct)
	{
		var table = await GetTable();

		IEnumerable<SpotifyArtistReleaseEntity>? linksByReleases;
		ct.ThrowIfCancellationRequested();

		if (artistRole.HasValue && releaseType.HasValue)
		{
			var links = releaseIds.Select(id => (id, artistRole.Value, releaseType.Value));
			linksByReleases = await table.Where(x => x.ReleaseId, x => x.Role, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}
		else if (artistRole.HasValue)
		{
			var links = releaseIds.Select(id => (id, artistRole.Value));
			linksByReleases = await table.Where(x => x.ReleaseId, x => x.Role).AnyOf([.. links]).ToArray();
		}
		else if (releaseType.HasValue)
		{
			var links = releaseIds.Select(id => (id, releaseType.Value));
			linksByReleases = await table.Where(x => x.ReleaseId, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}
		else
		{
			var links = releaseIds;
			linksByReleases = await table.Where(x => x.ReleaseId).AnyOf([.. links]).ToArray();
		}

		return linksByReleases;
	}
}