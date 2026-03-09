using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Mappers.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Data;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyReleaseService(IDbSpotifyService dbService, IDbSpotifyArtistReleaseService linkArtistDb, IDbSpotifyArtistService artistsDb) : IDbSpotifyReleaseService
{
	private readonly IDbSpotifyService _dbService = dbService;
	private readonly IDbSpotifyArtistReleaseService _linkArtistDb = linkArtistDb;
	private readonly IDbSpotifyArtistService _artistDb = artistsDb;

	public async Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, MainReleasesType mainReleaseType)
	{
		Console.WriteLine("db: get releases by ids - start");

		var db = await _dbService.GetDb();

		// get releases
		IEnumerable<SpotifyReleaseEntity> releasesDb;

		if (mainReleaseType == MainReleasesType.Appears)
		{
			releasesDb = await db.Release.BulkGet(ids);
		}
		else
		{
			var releaseType = EnumReleaseTypeExtensions.MapFromMain(mainReleaseType);
			releasesDb = await db.Release.Where(x => x.Id, x => x.ReleaseType).AnyOf([.. ids.Select(id => (id, releaseType))]).ToArray();
		}

		if (!releasesDb.Any())
		{
			return [];
		}

		var releaseIds = releasesDb.Select(x => x.Id).ToArray();

		// get links
		var allLinks = await _linkArtistDb.GetByReleaseIds(releaseIds);

		// get artists
		var artistIds = allLinks.Select(l => l.ArtistId).ToHashSet();
		var artists = await _artistDb.GetByIds(artistIds);
		var artistsDict = artists.ToDictionary(a => a.Id);

		// map releases
		var linksByRelease = allLinks.ToLookup(l => l.ReleaseId);

		var releases = new List<SpotifyRelease>(releasesDb.Count());

		foreach (var releaseDb in releasesDb)
		{
			var releaseLinks = linksByRelease[releaseDb.Id];

			var mainArtists = new HashSet<SpotifyArtist>();
			var featuredArtists = new HashSet<SpotifyArtist>();

			foreach (var link in releaseLinks)
			{
				if (artistsDict.TryGetValue(link.ArtistId, out var artist))
				{
					if (link.Role == ArtistReleaseRole.Main)
					{
						mainArtists.Add(artist);
					}
					else if (link.Role == ArtistReleaseRole.Featured)
					{
						featuredArtists.Add(artist);
					}
				}
			}

			var release = releaseDb.ToModel(mainArtists, featuredArtists);
			releases.Add(release);
		}

		Console.WriteLine("db: get releases by ids - end");
		return releases;
	}


	public async Task Save(IReadOnlyList<SpotifyRelease> releases)
	{
		Console.WriteLine("db: save releases - start");

		if (releases.Count == 0)
		{
			return;
		}

		var db = await _dbService.GetDb();

		var incomingIds = releases.Select(r => r.Id).ToArray();
		var existingIds = await db.Release.Where(x => x.Id).AnyOf(incomingIds).Keys();
		var existingIdsSet = existingIds.ToHashSet();

		var releasesDb = releases.Where(r => !existingIdsSet.Contains(r.Id)).Select(a => a.ToEntity()).ToList();

		await db.Release.BulkPutSafe(releasesDb);

		Console.WriteLine("db: save releases - end");
	}

	public async Task Add(SpotifyRelease release)
	{
		Console.WriteLine("db: add release - start");
		var db = await _dbService.GetDb();
		var releaseDb = release.ToEntity();
		await db.Release.PutSafe(releaseDb);
		Console.WriteLine("db: add release - end");
	}
}