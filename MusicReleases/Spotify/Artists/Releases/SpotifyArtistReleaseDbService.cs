using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Spotify.Artists.Releases;

internal sealed class SpotifyArtistReleaseDbService(IDbSpotifyService dbService) : ISpotifyArtistReleaseDbService
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
