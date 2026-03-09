using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Mappers.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyArtistReleaseService(IDbSpotifyService dbService) : IDbSpotifyArtistReleaseService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<HashSet<string>> GetReleaseIds(string artistId, ArtistReleaseRole artistRole)
	{
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ArtistId, artistId).And(x => x.Role == artistRole).ToArray();

		return links.Select(x => x.ReleaseId).ToHashSet();
	}

	public async Task<HashSet<SpotifyArtistReleaseEntity>> GetByReleaseIds(IEnumerable<string> releaseIds)
	{
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId).AnyOf([.. releaseIds]).ToArray();

		return links.ToHashSet();
	}

	public async Task<HashSet<string>> GetReleaseIds(IEnumerable<string> artistIds, ArtistReleaseRole artistRole)
	{
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ArtistId).AnyOf([.. artistIds]).And(x => x.Role == artistRole).ToArray();

		return links.Select(x => x.ReleaseId).ToHashSet();
	}

	public async Task<HashSet<string>> GetArtistIds(string releaseId, ArtistReleaseRole artistRole)
	{
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId, releaseId).And(x => x.Role == artistRole).ToArray();

		return links.Select(x => x.ArtistId).ToHashSet();
	}

	public async Task<HashSet<string>> GetArtistIds(string releaseId)
	{
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId, releaseId).ToArray();

		return links.Select(x => x.ArtistId).ToHashSet();
	}


	public async Task SetArtistReleases(string artistId, MainReleasesType mainReleaseType, IEnumerable<string> releaseApiIdsEnumerable)
	{
		var artistRole = EnumReleaseTypeExtensions.MapReleaseRole(mainReleaseType);

		var db = await _dbService.GetDb();

		var apiIds = releaseApiIdsEnumerable.ToList();
		var currentIds = await GetReleaseIds(artistId, artistRole);

		// remove old
		var otherArtistRoleReleaseIds = currentIds.Except(apiIds).ToArray();
		if (otherArtistRoleReleaseIds.Length > 0)
		{
			var releasesDb = await db.Release.Where(x => x.Id).AnyOf(otherArtistRoleReleaseIds).ToArray();

			if (mainReleaseType != MainReleasesType.Appears)
			{
				var releaseType = EnumReleaseTypeExtensions.MapFromMain(mainReleaseType);
				releasesDb = releasesDb.Where(x => x.ReleaseType == releaseType);
			}
			var toRemoveIds = releasesDb.Select(x => x.Id);
			if (toRemoveIds.Any())
			{
				await db.ArtistRelease.Where(x => x.ArtistId, artistId).Filter(x => toRemoveIds.Contains(x.ReleaseId)).Delete();
			}
		}

		// add new
		var currentArtistRoleReleaseIds = apiIds.Except(currentIds).Select(rid => rid.ToArtistReleaseEntity(artistId, artistRole)).ToList();
		if (currentArtistRoleReleaseIds.Count > 0)
		{
			await db.ArtistRelease.BulkPutSafe(currentArtistRoleReleaseIds);
		}
	}

	public async Task Save(IEnumerable<SpotifyArtistReleaseEntity> links)
	{
		var db = await _dbService.GetDb();
		await db.ArtistRelease.BulkPutSafe(links);
	}

	public async Task SetArtistReleases(IEnumerable<SpotifyRelease> releases, ArtistReleaseRole artistRole)
	{
		var db = await _dbService.GetDb();
		var linksToAdd = new List<SpotifyArtistReleaseEntity>();

		foreach (var release in releases)
		{
			var artists = artistRole == ArtistReleaseRole.Main ? release.FeaturedArtists : release.Artists;
			var artistRoleSave = artistRole == ArtistReleaseRole.Main ? ArtistReleaseRole.Featured : ArtistReleaseRole.Main;
			foreach (var artist in artists)
			{
				var link = release.Id.ToArtistReleaseEntity(artist.Id, artistRoleSave);
				linksToAdd.Add(link);
			}
		}
		await db.ArtistRelease.BulkPutSafe(linksToAdd);
	}

	public async Task DeleteAllForArtist(string artistId)
	{
		var db = await _dbService.GetDb();
		await db.ArtistRelease.Where(x => x.ArtistId, artistId).Delete();
	}


	public async Task AddArtistRelease(string artistId, string releaseId, ArtistReleaseRole artistRole)
	{
		var db = await _dbService.GetDb();
		var playlistDb = releaseId.ToArtistReleaseEntity(artistId, artistRole);
		await db.ArtistRelease.PutSafe(playlistDb);
	}
}
