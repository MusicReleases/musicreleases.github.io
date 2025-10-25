using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyArtistReleaseService(IDbSpotifyService dbService, IDbSpotifyReleaseService dbReleaseService, IDbSpotifyUpdateService dbUpdateService, IDbSpotifyArtistService dbArtistService) : IDbSpotifyArtistReleaseService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyArtistRelease);

	private readonly IDbSpotifyReleaseService _dbReleaseService = dbReleaseService;
	private readonly IDbSpotifyUpdateService _dbUpdateService = dbUpdateService;
	private readonly IDbSpotifyArtistService _dbArtistService = dbArtistService;

	public async Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>?> Get(ISet<SpotifyArtist> artists, string userId)
	{
		// update
		var update = await GetUpdateDb(userId);
		if (update is null)
		{
			return null;
		}

		// artists
		var releaseIds = await GetReleaseIds(artists);
		if (releaseIds.Count < 1)
		{
			return null;
		}

		var releases = await _dbReleaseService.Get(releaseIds);
		if (releases is null)
		{
			return null;
		}

		var releasesUpdate = new SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>(releases, update);

		return releasesUpdate;
	}
	private async Task<SpotifyUserListUpdateRelease?> GetUpdateDb(string userId)
	{
		var updateDb = await _dbUpdateService.Get(userId);
		if (updateDb is null)
		{
			// TODO delete user artists
			return null;
		}

		// TODO change SpotifyUserListUpdateArtists
		var update = new SpotifyUserListUpdateRelease
		{
			LastUpdateAlbums = updateDb.ReleasesAlbums,
			LastUpdateTracks = updateDb.ReleasesTracks,
			LastUpdateAppears = updateDb.ReleasesAppears,
			LastUpdateCompilations = updateDb.ReleasesCompilations,
			LastUpdatePodcasts = updateDb.ReleasesPodcasts,
		};

		return update;
	}

	private async Task<ISet<SpotifyReleaseArtistsDbObject>> GetReleaseIds(ISet<SpotifyArtist> followedArtists)
	{
		var artistReleasesDb = await GetAllDb();
		var allArtists = await _dbArtistService.GetAll() ?? throw new NullReferenceException("allArtists");

		var artistDict = allArtists.ToDictionary(a => a.Id);

		var followedReleases = PrepareFollowedReleases(artistReleasesDb, followedArtists);

		var releaseIds = BuildReleaseArtistObjects(followedReleases, artistDict);

		return releaseIds;
	}

	private Dictionary<string, HashSet<string>> PrepareFollowedReleases(IEnumerable<SpotifyArtistReleaseEntity> allReleases, ISet<SpotifyArtist> followedArtists)
	{
		return allReleases
			.Where(x => followedArtists.Any(y => y.Id == x.ArtistId))
			.GroupBy(x => x.ReleaseId)
			.ToDictionary(g => g.Key, g => g.Select(a => a.ArtistId).ToHashSet());
	}

	private ISet<SpotifyReleaseArtistsDbObject> BuildReleaseArtistObjects(Dictionary<string, HashSet<string>> releaseArtistMap, Dictionary<string, SpotifyArtist> artistDict)
	{
		var releaseArtistsDb = new HashSet<SpotifyReleaseArtistsDbObject>();

		foreach (var release in releaseArtistMap)
		{
			var artists = release.Value
				.Where(artistDict.ContainsKey)
				.Select(id => artistDict[id])
				.ToHashSet();

			releaseArtistsDb.Add(new SpotifyReleaseArtistsDbObject(release.Key, artists));
		}

		return releaseArtistsDb;
	}


	private async Task<ISet<SpotifyArtistReleaseEntity>> GetAllDb()
	{
		Console.WriteLine("db: get artist releases - start");
		var artistReleasesDb = _dbTable.GetAllAsync<SpotifyArtistReleaseEntity>();

		var artistReleases = new HashSet<SpotifyArtistReleaseEntity>();
		await foreach (var artistReleaseDb in artistReleasesDb)
		{
			artistReleases.Add(artistReleaseDb);
		}

		Console.WriteLine("db: get artist releases - end");
		return artistReleases;
	}

	public async Task Save(string userId, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease> releases)
	{
		// TODO remove unfollowed artists and deleted releases

		if (releases.Update is null)
		{
			// TODO
			throw new NullReferenceException(nameof(releases.Update));
		}
		if (releases.List is null)
		{
			// TODO
			throw new NullReferenceException(nameof(releases.List));
		}

		// update db
		await SaveUpdateDb(userId, releases.Update);

		// user releases db
		await SaveDb(releases.List, userId);
	}

	private async Task SaveUpdateDb(string userId, SpotifyUserListUpdateRelease update)
	{
		var updateDb = await _dbUpdateService.Get(userId);

		if (updateDb is null)
		{
			// TODO
			throw new NullReferenceException(nameof(updateDb));
		}

		// update - update times
		updateDb.ReleasesAlbums = update.LastUpdateAlbums;
		updateDb.ReleasesTracks = update.LastUpdateTracks;
		updateDb.ReleasesAppears = update.LastUpdateAppears;
		updateDb.ReleasesCompilations = update.LastUpdateCompilations;
		updateDb.ReleasesPodcasts = update.LastUpdatePodcasts;

		await _dbUpdateService.Update(updateDb);
	}


	private async Task SaveDb(ISet<SpotifyRelease> releases, string userId)
	{
		Console.WriteLine("db: save artist releases - start");
		var artistReleasesDb = await GetAllDb();
		var newReleases = releases.Where(x => !artistReleasesDb.Any(y => y.ReleaseId == x.Id)).ToHashSet();
		var newArtists = new HashSet<SpotifyArtist>();

		// save new releases
		await _dbReleaseService.Save(newReleases);
		foreach (var release in newReleases)
		{
			foreach (var artist in release.Artists)
			{
				var artistReleaseEntity = new SpotifyArtistReleaseEntity(artist.Id, release.Id);
				await _dbTable.StoreAsync(artistReleaseEntity);
				newArtists.Add(artist);
			}
		}

		// save release artists
		await _dbArtistService.Save(newArtists);
		Console.WriteLine("db: save artist releases - end");

		// delete or keep releases
		var artists = releases.SelectMany(x => x.Artists);

		// keep releases from unfollowed artists (for cache)
		var artistReleasesFromFollowedArtists = artistReleasesDb.Where(x => artists.Any(y => y.Id == x.ArtistId));
		var deletedReleases = artistReleasesFromFollowedArtists.Where(x => !releases.Any(y => y.Id == x.ReleaseId)).ToHashSet();

		// delete releases from followed artists
		await Delete(deletedReleases);
	}

	private async Task Delete(ISet<SpotifyArtistReleaseEntity> artistReleasesDb)
	{
		Console.WriteLine("db: delete artist releases - start");
		foreach (var artistReleaseDb in artistReleasesDb)
		{
			// delete releases from followed artists (= probably deleted from spotify)
			await _dbTable.RemoveItemAsync(artistReleaseDb);
			await _dbReleaseService.Delete(artistReleaseDb.ReleaseId);
		}
		Console.WriteLine("db: dele artist releases - end");
	}
}
