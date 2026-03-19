using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases.Artists;

internal sealed class SpotifyArtistReleaseDbService(IDbSpotifyService dbService) : SpotifyArtistLinkService<SpotifyArtistReleaseEntity>, ISpotifyArtistReleaseDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override async Task<IEnumerable<SpotifyArtistReleaseEntity>> FetchByKeys1(IEnumerable<string> artistIds)
	{
		var table = await GetTable();

		var linksByArtists = await table.Where(x => x.ArtistId).AnyOf([.. artistIds]).ToArray();

		return linksByArtists;
	}

	protected override async Task<IEnumerable<SpotifyArtistReleaseEntity>> FetchByKeys2(IEnumerable<string> releaseIds)
	{
		var table = await GetTable();

		var linksByReleases = await table.Where(x => x.ReleaseId).AnyOf([.. releaseIds]).ToArray();

		return linksByReleases;
	}


	protected override async Task<IEnumerable<SpotifyArtistReleaseEntity>> FetchByArtistsAndGroup(IEnumerable<string> artistIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType)
	{
		var table = await GetTable();

		IEnumerable<SpotifyArtistReleaseEntity>? linksByReleases;

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
	protected override async Task<IEnumerable<SpotifyArtistReleaseEntity>> FetchByReleasesAndGroup(IEnumerable<string> releaseIds, ArtistReleaseRole? artistRole, ReleaseType? releaseType)
	{
		var table = await GetTable();

		IEnumerable<SpotifyArtistReleaseEntity>? linksByReleases;

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

	protected override async Task<Table<SpotifyArtistReleaseEntity, (string, string)>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.ArtistRelease;
	}
}

internal sealed class SpotifyArtistReleaseDbService2(IDbSpotifyService dbService) : ISpotifyArtistReleaseDbServiceOld
{
	// TODO check and optimize

	private readonly IDbSpotifyService _dbService = dbService;
	/*
	public async Task<HashSet<string>> GetReleaseIds(string artistId, ArtistReleaseRole artistRole, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		var links = await db.ArtistRelease.Where(x => x.ArtistId, artistId, x => x.Role, artistRole).ToArray();

		return links.Select(x => x.ReleaseId).ToHashSet();
	}*/

	public async Task<HashSet<SpotifyArtistReleaseEntity>> GetByReleaseIds(IEnumerable<string> releaseIds, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId).AnyOf([.. releaseIds]).ToArray();

		return links.ToHashSet();
	}

	public async Task<HashSet<string>> GetReleaseIds(IEnumerable<string> artistIds, ArtistReleaseRole artistRole, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		var links = await db.ArtistRelease.Where(x => x.ArtistId, x => x.Role).AnyOf([.. artistIds.Select(id => (id, artistRole))]).ToArray();

		return links.Select(x => x.ReleaseId).ToHashSet();
	}
	/*
	public async Task<HashSet<string>> GetArtistIds(string releaseId, ArtistReleaseRole artistRole, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId, releaseId, x => x.Role, artistRole).ToArray();

		return links.Select(x => x.ArtistId).ToHashSet();
	}

	public async Task<HashSet<string>> GetArtistIds(string releaseId, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId, releaseId).ToArray();

		return links.Select(x => x.ArtistId).ToHashSet();
	}

	/*
	public async Task SetArtistReleases(string artistId, ReleaseEnums mainReleaseType, IEnumerable<string> releaseApiIdsEnumerable, CancellationToken ct)
	{
		var artistRole = EnumReleaseTypeExtensions.MapReleaseRoleFromGroup(mainReleaseType);
		var apiIds = releaseApiIdsEnumerable.ToList();
		var currentIds = await GetReleaseIds(artistId, artistRole, ct);

		var db = await _dbService.GetDb();

		// remove old
		var otherArtistRoleReleaseIds = currentIds.Except(apiIds).ToArray();
		if (otherArtistRoleReleaseIds.Length > 0)
		{
			ct.ThrowIfCancellationRequested();
			var releasesDb = await db.Release.Where(x => x.Id).AnyOf(otherArtistRoleReleaseIds).ToArray();
			ct.ThrowIfCancellationRequested();

			if (mainReleaseType != ReleaseEnums.Appears)
			{
				var releaseType = EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(mainReleaseType);
				releasesDb = releasesDb.Where(x => x.ReleaseType == releaseType);
			}
			var toRemoveIds = releasesDb.Select(x => x.Id);
			if (toRemoveIds.Any())
			{
				ct.ThrowIfCancellationRequested();
				await db.ArtistRelease.Where(x => x.ArtistId, artistId).Filter(x => toRemoveIds.Contains(x.ReleaseId)).Delete();
			}
		}

		// add new
		var currentArtistRoleReleaseIds = apiIds.Except(currentIds).Select(rid => rid.ToArtistReleaseEntity(artistId, artistRole)).ToList();
		if (currentArtistRoleReleaseIds.Count > 0)
		{
			ct.ThrowIfCancellationRequested();
			await db.ArtistRelease.BulkPutSafe(currentArtistRoleReleaseIds);
		}
	}*/

	public async Task Save(IEnumerable<SpotifyArtistReleaseEntity> links, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		await db.ArtistRelease.BulkPutSafe(links);

	}
	/*
	public async Task SetArtistReleases(IEnumerable<SpotifyRelease> releases, ArtistReleaseRole artistRole, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		var linksToAdd = new List<SpotifyArtistReleaseEntity>();

		foreach (var release in releases)
		{
			ct.ThrowIfCancellationRequested();

			var artists = artistRole == ArtistReleaseRole.Main ? release.FeaturedArtists : release.Artists;
			var artistRoleSave = artistRole == ArtistReleaseRole.Main ? ArtistReleaseRole.Featured : ArtistReleaseRole.Main;

			foreach (var artist in artists)
			{
				ct.ThrowIfCancellationRequested();
				var link = release.Id.ToArtistReleaseEntity(artist.Id, artistRoleSave);
				linksToAdd.Add(link);
			}
		}

		ct.ThrowIfCancellationRequested();
		await db.ArtistRelease.BulkPutSafe(linksToAdd);
	}*/
	/*
	public async Task DeleteAllForArtist(string artistId, CancellationToken ct)
	{
		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		await db.ArtistRelease.Where(x => x.ArtistId, artistId).Delete();
	}
	*/
	/*
	public async Task AddArtistRelease(string artistId, string releaseId, ArtistReleaseRole artistRole, CancellationToken ct)
	{
		var playlistDb = releaseId.ToArtistReleaseEntity(artistId, artistRole);

		var db = await _dbService.GetDb();
		ct.ThrowIfCancellationRequested();

		await db.ArtistRelease.PutSafe(playlistDb);
	}*/
}
