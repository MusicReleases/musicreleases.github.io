using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.SpotifyApi.Releases;
using System.Data;

namespace JakubKastner.MusicReleases.Spotify.Releases;


internal sealed class SpotifyReleaseDbService(IDbSpotifyService dbService) : SpotifyIdEntityService<SpotifyRelease, SpotifyReleaseEntity, SpotifyArtistReleasePayload>, ISpotifyReleaseDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	private ReleaseGroup _currentReleaseGroup;

	protected override SpotifyReleaseEntity ToEntity(SpotifyRelease model) => model.ToEntity();

	protected override SpotifyRelease ToModel(SpotifyReleaseEntity entity, SpotifyArtistReleasePayload payload) => entity.ToModel(payload.MainArtists, payload.FeaturedArtists);

	protected override async Task<Table<SpotifyReleaseEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.Release;
	}

	public Task<IReadOnlyCollection<SpotifyRelease>> GetByIds(IReadOnlyCollection<SpotifyArtistReleasePayload> payloads, ReleaseGroup releaseGroup, CancellationToken ct)
	{
		_currentReleaseGroup = releaseGroup;
		return GetByIds(payloads, ct);
	}

	protected override async Task<IEnumerable<SpotifyReleaseEntity>> FetchByIds(string[] ids)
	{
		var table = await GetTable();

		if (_currentReleaseGroup == ReleaseGroup.Appears)
		{
			return await table.BulkGet(ids);
		}

		var releaseType = EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(_currentReleaseGroup);

		var keys = ids.Select(id => (id, releaseType)).ToArray();

		return await table.Where(x => x.Id, x => x.ReleaseType).AnyOf(keys).ToArray();
	}
}

/*
internal sealed class SpotifyReleaseDbService2(IDbSpotifyService dbService, ISpotifyArtistReleaseDbService linkArtistDb, ISpotifyArtistDbService artistsDb)
{
	private readonly IDbSpotifyService _dbService = dbService;
	private readonly ISpotifyArtistReleaseDbService _linkArtistDb = linkArtistDb;
	private readonly ISpotifyArtistDbService _artistDb = artistsDb;

	public async Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, ReleaseGroup releaseGroup, CancellationToken ct)
	{
		var db = await _dbService.GetDb();

		// get releases
		IEnumerable<SpotifyReleaseEntity> releasesDb;

		if (releaseGroup == ReleaseGroup.Appears)
		{
			ct.ThrowIfCancellationRequested();
			releasesDb = await db.Release.BulkGet(ids);
		}
		else
		{
			var releaseType = EnumReleaseTypeExtensions.MapReleaseTypeFromGroup(releaseGroup);

			ct.ThrowIfCancellationRequested();
			releasesDb = await db.Release.Where(x => x.Id, x => x.ReleaseType).AnyOf([.. ids.Select(id => (id, releaseType))]).ToArray();
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

	public async Task Save(SpotifyRelease release, CancellationToken ct)
	{
		Console.WriteLine("db: add release - start");

		var db = await _dbService.GetDb();
		var releaseDb = release.ToEntity();

		ct.ThrowIfCancellationRequested();
		await db.Release.PutSafe(releaseDb);


		Console.WriteLine("db: add release - end");
	}
}*/