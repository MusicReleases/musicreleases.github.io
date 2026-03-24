using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services.Links;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases.Artists;

internal sealed class SpotifyArtistReleaseDbService(IDbSpotifyService dbService) : SpotifyArtistLinkEntityService<SpotifyArtistReleaseEntity>, ISpotifyArtistReleaseDbService
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

		ReleaseType? releaseType;

		if (releaseGroup == ReleaseGroup.Appears)
		{
			releaseType = null;
		}
		else
		{
			releaseType = EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(releaseGroup);
		}

		var releases = await GetByArtists(artistIds, artistRole, releaseType, ct);

		var releaseIds = releases.Select(x => x.ReleaseId).ToHashSet();

		var missingReleaseIds = releaseIds.Where(id => !_cache.ContainsKey(id)).ToList();

		if (missingReleaseIds.Count > 0)
		{
			var artists = await GetByReleases(missingReleaseIds, null, releaseType, ct);

			var payloads = artists
				.GroupBy(x => x.ReleaseId)
				.Select(g => new SpotifyArtistGroupByReleasePayload(
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

		var result = new List<SpotifyArtistGroupByReleasePayload>(releaseIds.Count);

		foreach (var id in releaseIds)
		{
			if (_cache.TryGetValue(id, out var payload))
			{
				result.Add(payload);
			}
		}

		return result.AsReadOnly();
	}

	private async Task<IEnumerable<SpotifyArtistReleaseEntity>> GetByArtists(IEnumerable<string> artistIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType, CancellationToken ct)
	{
		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		if (artistRole.HasValue && releaseType.HasValue)
		{
			var links = artistIds.Select(id => (id, artistRole.Value, releaseType.Value));
			return await table.Where(x => x.ArtistId, x => x.Role, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}

		if (artistRole.HasValue)
		{
			var links = artistIds.Select(id => (id, artistRole.Value));
			return await table.Where(x => x.ArtistId, x => x.Role).AnyOf([.. links]).ToArray();
		}

		if (releaseType.HasValue)
		{
			var links = artistIds.Select(id => (id, releaseType.Value));
			return await table.Where(x => x.ArtistId, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}

		return await table.Where(x => x.ArtistId).AnyOf([.. artistIds]).ToArray();
	}

	private async Task<IEnumerable<SpotifyArtistReleaseEntity>> GetByReleases(IEnumerable<string> releaseIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType, CancellationToken ct)
	{
		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		if (artistRole.HasValue && releaseType.HasValue)
		{
			var links = releaseIds.Select(id => (id, artistRole.Value, releaseType.Value));
			return await table.Where(x => x.ReleaseId, x => x.Role, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}

		if (artistRole.HasValue)
		{
			var links = releaseIds.Select(id => (id, artistRole.Value));
			return await table.Where(x => x.ReleaseId, x => x.Role).AnyOf([.. links]).ToArray();
		}

		if (releaseType.HasValue)
		{
			var links = releaseIds.Select(id => (id, releaseType.Value));
			return await table.Where(x => x.ReleaseId, x => x.ReleaseType).AnyOf([.. links]).ToArray();
		}

		return await table.Where(x => x.ReleaseId).AnyOf([.. releaseIds]).ToArray();
	}
}