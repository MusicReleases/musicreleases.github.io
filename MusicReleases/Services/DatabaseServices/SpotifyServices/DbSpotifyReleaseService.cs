using DexieNET;
using JakubKastner.MusicReleases.Mappers.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyReleaseService(IDbSpotifyService dbService, IDbSpotifyArtistReleaseService linkArtistDb, IDbSpotifyArtistService artistsDb) : IDbSpotifyReleaseService
{
	private readonly IDbSpotifyService _dbService = dbService;
	private readonly IDbSpotifyArtistReleaseService _linkArtistDb = linkArtistDb;
	private readonly IDbSpotifyArtistService _artistDb = artistsDb;

	/*public async Task<IReadOnlyList<SpotifyRelease>?> GetAll()
	{
		Console.WriteLine("db: get all releases - start");

		var db = await _dbService.GetDb();
		var releasesDb = await db.Release.ToArray();

		var releases = releasesDb.Select(e => e.ToModel()).ToList();

		Console.WriteLine($"db: get all releases - end");
		return releases;
	}*/

	/*public async Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, MainReleasesType releaseType)
	{
		Console.WriteLine("db: get releases by ids - start");

		var db = await _dbService.GetDb();

		var releases = new List<SpotifyRelease>();
		var releasesDb = await db.Release.BulkGet(ids);

		foreach (var releaseDb in releasesDb)
		{
			var mainArtistIds = await _linkArtistDb.GetArtistIds(releaseDb.Id, ArtistReleaseRole.Main);
			var featuredArtistIds = await _linkArtistDb.GetArtistIds(releaseDb.Id, ArtistReleaseRole.Featured);

			var artistIds = mainArtistIds;
			artistIds.UnionWith(featuredArtistIds);

			var artists = await _artistDb.GetByIds(artistIds);
			var mainArtists = artists.Where(a => mainArtistIds.Contains(a.Id)).ToHashSet();
			var featuredArtists = artists.Where(a => featuredArtistIds.Contains(a.Id)).ToHashSet();

			var release = releaseDb.ToModel(mainArtists, featuredArtists);
		}

		Console.WriteLine($"db: get releases by ids - end");
		return releases;
	}*/

	public async Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, MainReleasesType mainReleaseType)
	{
		Console.WriteLine("db: get releases by ids - start");

		var db = await _dbService.GetDb();

		var releasesDb = await db.Release.BulkGet(ids);
		var filteredReleasesDb
			= mainReleaseType == MainReleasesType.Appears
			? releasesDb
			: releasesDb.Where(r => r.ReleaseType == EnumReleaseTypeExtensions.MapFromMain(mainReleaseType));

		var releaseIds = filteredReleasesDb.Select(x => x.Id).ToArray();


		// get links
		var allLinks = await db.ArtistRelease.Where(x => x.ReleaseId).AnyOf(releaseIds).ToArray();

		// get artists
		var artistIds = allLinks.Select(l => l.ArtistId).ToHashSet();
		var artists = await _artistDb.GetByIds(artistIds);
		var artistsDict = artists.ToDictionary(a => a.Id);

		var xyz = allLinks.Where(x => x.Role == ArtistReleaseRole.Main).ToList();

		// map releases
		var releaseRole = EnumReleaseTypeExtensions.MapReleaseRole(mainReleaseType);
		var releases = new List<SpotifyRelease>();

		foreach (var releaseDb in filteredReleasesDb)
		{
			/*var releaseLinks = allLinks.Where(l => l.Role == releaseRole && l.ReleaseId == releaseDb.Id);

			if (!releaseLinks.Any())
			{
				continue;
			}*/

			var releaseLinks = allLinks.Where(l => l.ReleaseId == releaseDb.Id);

			var mainArtistIds = releaseLinks.Where(l => l.Role == ArtistReleaseRole.Main).Select(l => l.ArtistId);
			var featuredArtistIds = releaseLinks.Where(l => l.Role == ArtistReleaseRole.Featured).Select(l => l.ArtistId);

			var mainArtists = mainArtistIds.Select(id => artistsDict[id]).ToHashSet();
			var featuredArtists = featuredArtistIds.Select(id => artistsDict[id]).ToHashSet();

			var release = releaseDb.ToModel(mainArtists, featuredArtists);
			releases.Add(release);
		}

		Console.WriteLine($"db: get releases by ids - end");
		return releases;
	}

	public async Task Save(IReadOnlyList<SpotifyRelease> releases)
	{
		Console.WriteLine("db: save releases - start");

		if (releases.Count == 0)
		{
			return;
		}

		var releasesDb = releases.Select(a => a.ToEntity());

		var db = await _dbService.GetDb();
		await db.Release.BulkPutSafe(releasesDb);

		Console.WriteLine("db: save releases - end");
	}

	public async Task Add(SpotifyRelease release)
	{
		var db = await _dbService.GetDb();
		var releaseDb = release.ToEntity();
		await db.Release.PutSafe(releaseDb);
	}
}