using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyArtistReleaseService(IDbSpotifyService dbService) : IDbSpotifyArtistReleaseService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<HashSet<string>> GetReleaseIds(string artistId, ArtistReleaseRole artistRole)
	{
		Console.WriteLine("db: get all artist-release (releaseIds) by artist - start");
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ArtistId, artistId, x => x.Role, artistRole).ToArray();

		Console.WriteLine("db: get all artist-release (releaseIds) by artist - end");
		return links.Select(x => x.ReleaseId).ToHashSet();
	}

	public async Task<HashSet<SpotifyArtistReleaseEntity>> GetByReleaseIds(IEnumerable<string> releaseIds)
	{
		Console.WriteLine("db: get all artist-release (entity) by ids - start");
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId).AnyOf([.. releaseIds]).ToArray();

		Console.WriteLine("db: get all artist-release (entity) by ids - end");
		return links.ToHashSet();
	}

	public async Task<HashSet<string>> GetReleaseIds(IEnumerable<string> artistIds, ArtistReleaseRole artistRole)
	{
		Console.WriteLine("db: get all artist-release (releaseIds) by artists - start");
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ArtistId, x => x.Role).AnyOf([.. artistIds.Select(id => (id, artistRole))]).ToArray();

		Console.WriteLine("db: get all artist-release (releaseIds) by artists - end");
		return links.Select(x => x.ReleaseId).ToHashSet();
	}

	public async Task<HashSet<string>> GetArtistIds(string releaseId, ArtistReleaseRole artistRole)
	{
		Console.WriteLine("db: get all artist-release (artistId) by releaseId - start");
		var db = await _dbService.GetDb();

		var links = await db.ArtistRelease.Where(x => x.ReleaseId, releaseId, x => x.Role, artistRole).ToArray();

		Console.WriteLine("db: get all artist-release (artistId) by releaseId - end");
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
		Console.WriteLine("db: set artist-release - start");
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
		Console.WriteLine("db: set artist-release - end");
	}

	public async Task Save(IEnumerable<SpotifyArtistReleaseEntity> links)
	{
		Console.WriteLine("db: set artist-release - save links - start");
		var db = await _dbService.GetDb();
		await db.ArtistRelease.BulkPutSafe(links);
		Console.WriteLine("db: set artist-release - save links - start");
	}

	public async Task SetArtistReleases(IEnumerable<SpotifyRelease> releases, ArtistReleaseRole artistRole)
	{
		Console.WriteLine("db: set artist-release - by releases - start");
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
		Console.WriteLine("db: set artist-release - by releases - end");
	}

	public async Task DeleteAllForArtist(string artistId)
	{
		Console.WriteLine("db: delete artist-release - by artist id - start");
		var db = await _dbService.GetDb();
		await db.ArtistRelease.Where(x => x.ArtistId, artistId).Delete();
		Console.WriteLine("db: delete artist-release - by artist id - end");
	}


	public async Task AddArtistRelease(string artistId, string releaseId, ArtistReleaseRole artistRole)
	{
		Console.WriteLine("db: add artist-release - start");
		var db = await _dbService.GetDb();
		var playlistDb = releaseId.ToArtistReleaseEntity(artistId, artistRole);
		await db.ArtistRelease.PutSafe(playlistDb);
		Console.WriteLine("db: add artist-release - end");
	}
}
