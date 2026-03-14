using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;
using System.Data;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyReleaseService(IDbSpotifyService dbService, IDbSpotifyArtistReleaseService linkArtistDb, IDbSpotifyArtistService artistsDb) : IDbSpotifyReleaseService
{
	private readonly IDbSpotifyService _dbService = dbService;
	private readonly IDbSpotifyArtistReleaseService _linkArtistDb = linkArtistDb;
	private readonly IDbSpotifyArtistService _artistDb = artistsDb;

	public async Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, ReleaseGroup mainReleaseType, CancellationToken ct)
	{
		Console.WriteLine("db: get releases by ids - start");

		var db = await _dbService.GetDb();

		// get releases
		IEnumerable<SpotifyReleaseEntity> releasesDb;

		if (mainReleaseType == ReleaseGroup.Appears)
		{
			ct.ThrowIfCancellationRequested();
			releasesDb = await db.Release.BulkGet(ids);
			ct.ThrowIfCancellationRequested();
		}
		else
		{
			var releaseType = EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(mainReleaseType);

			ct.ThrowIfCancellationRequested();
			releasesDb = await db.Release.Where(x => x.Id, x => x.ReleaseType).AnyOf([.. ids.Select(id => (id, releaseType))]).ToArray();
			ct.ThrowIfCancellationRequested();
		}

		if (!releasesDb.Any())
		{
			return [];
		}

		var releaseIds = releasesDb.Select(x => x.Id).ToArray();

		// get links
		ct.ThrowIfCancellationRequested();
		var allLinks = await _linkArtistDb.GetByReleaseIds(releaseIds, ct);
		ct.ThrowIfCancellationRequested();

		// get artists
		var artistIds = allLinks.Select(l => l.ArtistId).ToHashSet();
		var artists = await _artistDb.GetByIds(artistIds, ct);
		var artistsDict = artists.ToDictionary(a => a.Id);

		// map releases
		var linksByRelease = allLinks.ToLookup(l => l.ReleaseId);

		var releases = new List<SpotifyRelease>(releasesDb.Count());

		foreach (var releaseDb in releasesDb)
		{
			ct.ThrowIfCancellationRequested();
			var releaseLinks = linksByRelease[releaseDb.Id];

			var mainArtists = new HashSet<SpotifyArtist>();
			var featuredArtists = new HashSet<SpotifyArtist>();

			foreach (var link in releaseLinks)
			{
				ct.ThrowIfCancellationRequested();
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


	public async Task Save(IReadOnlyList<SpotifyRelease> releases, CancellationToken ct)
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

		ct.ThrowIfCancellationRequested();
		await db.Release.BulkPutSafe(releasesDb);

		Console.WriteLine("db: save releases - end");
	}

	public async Task Add(SpotifyRelease release, CancellationToken ct)
	{
		Console.WriteLine("db: add release - start");

		var db = await _dbService.GetDb();
		var releaseDb = release.ToEntity();

		ct.ThrowIfCancellationRequested();
		await db.Release.PutSafe(releaseDb);


		Console.WriteLine("db: add release - end");
	}
}