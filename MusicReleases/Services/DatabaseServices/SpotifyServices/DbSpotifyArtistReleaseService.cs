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
		var artistsDb = await _dbArtistService.GetAll() ?? throw new NullReferenceException("artistsDb");

		var artistDict = artistsDb.ToDictionary(a => a.Id);
		var followedArtistIds = followedArtists.Select(a => a.Id).ToHashSet();

		// releases from followed artist

		var releasesWithFollowed = artistReleasesDb
			.Where(ar => followedArtistIds.Contains(ar.ArtistId))
			.Select(ar => ar.ReleaseId)
			.ToHashSet();

		// releases from followed artist with all release artists
		var releaseArtistsDb = artistReleasesDb
			.Where(ar => releasesWithFollowed.Contains(ar.ReleaseId))
			.GroupBy(ar => ar.ReleaseId)
			.Select(g => new SpotifyReleaseArtistsDbObject(
				g.Key,
				g.Select(ar => artistDict.GetValueOrDefault(ar.ArtistId))
				 .Where(a => a is not null)
				 .Select(a => a!)
				 .ToHashSet()
			))
			.ToHashSet();

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

		var artistReleasesDb = await GetAllDb();

		var existingReleaseIds = artistReleasesDb.Select(x => x.ReleaseId).ToHashSet();
		var newReleases = releases.Where(x => !existingReleaseIds.Contains(x.Id)).ToHashSet();
		var newArtists = new HashSet<SpotifyArtist>();

		// save new releases
		await _dbReleaseService.Save(newReleases);

		var artistReleaseEntities = new HashSet<SpotifyArtistReleaseEntity>();
		foreach (var release in newReleases)
		{
			foreach (var artist in release.Artists)
			{
				var artistReleaseEntity = new SpotifyArtistReleaseEntity(artist.Id, release.Id);
				artistReleaseEntities.Add(artistReleaseEntity);
				newArtists.Add(artist);
			}
		}

		Console.WriteLine("db: save artist releases - start");
		var tasks = artistReleaseEntities.Select(e => _dbTable.StoreAsync(e));
		await Task.WhenAll(tasks);
		Console.WriteLine("db: save artist releases - end");

		// save release artists
		await _dbArtistService.Save(newArtists);


		// delete or keep releases
		var artists = releases.SelectMany(x => x.Artists).ToHashSet();

		var artistIds = artists.Select(a => a.Id).ToHashSet();
		var releaseIds = releases.Select(r => r.Id).ToHashSet();

		var artistReleasesFromFollowedArtists = artistReleasesDb.Where(x => artistIds.Contains(x.ArtistId)).ToHashSet();

		var deletedReleases = artistReleasesFromFollowedArtists.Where(x => !releaseIds.Contains(x.ReleaseId)).ToHashSet();

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
		Console.WriteLine("db: delete artist releases - end");
	}

	/*
private async Task Delete(ISet<SpotifyArtistReleaseEntity> artistReleasesDb)
{
	Console.WriteLine("db: delete artist releases - start");

	await Task.WhenAll(artistReleasesDb.Select(DeleteItem));

	Console.WriteLine("db: delete artist releases - end");
}

private async Task DeleteItem(SpotifyArtistReleaseEntity artistReleaseDb)
{
	// delete releases from followed artists (= probably deleted from spotify)
	await _dbTable.RemoveItemAsync(artistReleaseDb);
	await _dbReleaseService.Delete(artistReleaseDb.ReleaseId);
}
	*/
}
