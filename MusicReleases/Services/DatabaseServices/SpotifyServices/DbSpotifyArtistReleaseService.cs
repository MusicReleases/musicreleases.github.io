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
		// releases from followed artist
		var followedArtistReleasesDb = artistReleasesDb.Where(x => followedArtists.Any(y => y.Id == x.ArtistId));

		var allArtists = await _dbArtistService.GetAll();
		if (allArtists is null)
		{
			throw new NullReferenceException(nameof(allArtists));
		}

		var releaseArtistsDb = new HashSet<SpotifyReleaseArtistsDbObject>();

		foreach (var artistReleaseDb in followedArtistReleasesDb)
		{
			var releaseArtists = GetReleaseArtists(artistReleaseDb.ReleaseId, allArtists, artistReleasesDb);
			var dbObject = new SpotifyReleaseArtistsDbObject(artistReleaseDb.ReleaseId, releaseArtists);

			releaseArtistsDb.Add(dbObject);

		}

		return releaseArtistsDb;
	}
	private async Task<ISet<SpotifyArtistReleaseEntity>> GetAllDb()
	{
		var artistReleasesDb = _dbTable.GetAllAsync<SpotifyArtistReleaseEntity>();
		var artistReleases = new HashSet<SpotifyArtistReleaseEntity>();
		await foreach (var artistReleaseDb in artistReleasesDb)
		{
			artistReleases.Add(artistReleaseDb);
		}
		return artistReleases;
	}

	private ISet<SpotifyArtist> GetReleaseArtists(string releaseId, ISet<SpotifyArtist> allArtists, ISet<SpotifyArtistReleaseEntity> artistReleasesDb)
	{
		var releaseArtistIds = artistReleasesDb.Where(x => x.ReleaseId == releaseId).Select(x => x.ArtistId);

		var artists = new HashSet<SpotifyArtist>();
		foreach (var artistId in releaseArtistIds)
		{
			var artist = allArtists.First(x => x.Id == artistId);
			artists.Add(artist);
		}

		return artists;
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
		foreach (var artistReleaseDb in artistReleasesDb)
		{
			// delete releases from followed artists (= probably deleted from spotify)
			await _dbTable.RemoveItemAsync(artistReleaseDb);
			await _dbReleaseService.Delete(artistReleaseDb.ReleaseId);
		}
	}
}
