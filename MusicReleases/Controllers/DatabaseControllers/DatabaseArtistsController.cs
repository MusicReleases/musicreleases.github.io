using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseArtistsController(IDatabaseController dbController, IDatabaseUpdateController databaseUpdateController, IDatabaseReleasesController databaseReleasesController) : IDatabaseArtistsController
{
	private readonly IDatabaseController _dbController = dbController;

	private readonly IDatabaseUpdateController _databaseUpdateController = databaseUpdateController;
	private readonly IDatabaseReleasesController _databaseReleasesController = databaseReleasesController;

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> GetFollowed(string userId, bool getReleases)
	{
		// create db
		var db = _dbController.GetDb();

		// artists
		var artists = await GetFollowedDb(db, userId, getReleases);
		if (artists.Count < 1)
		{
			return null;
		}

		// update
		var update = await GetUpdateDb(db, userId);
		if (update is null)
		{
			return null;
		}

		var artistUpdate = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>(artists, update);

		return artistUpdate;
	}

	private async Task<ISet<SpotifyArtist>> GetFollowedDb(IndexedDb db, string userId, bool getReleases)
	{
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyArtist);

		// get artists from db
		Console.WriteLine("get artists");
		var artistsDb = table.GetAllAsync<SpotifyArtistEntity>();
		var artists = new HashSet<SpotifyArtist>();
		await foreach (var artistDb in artistsDb)
		{
			if (artistDb.Id.IsNullOrEmpty() || artistDb.Name.IsNullOrEmpty() || artistDb.Following)
			{
				continue;
			}

			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name);

			if (getReleases)
			{
				// get saved releases in db for current artist
				artist.Releases = await _databaseReleasesController.GetReleasesDb(db, artistDb.Id!, getReleases);
			}

			artists.Add(artist);
		}

		/*var query = table.Query<SpotifyArtistEntity>().Where(x => x.Id.IsNotNullOrEmpty() && x.Name.IsNotNullOrEmpty() && x.Following);

		// get artists from db
		var followedArtistsDb = await query.ToListAsync();

		var artists = new HashSet<SpotifyArtist>();
		foreach (var artistDb in followedArtistsDb)
		{
			var artist = new SpotifyArtist(artistDb.Id!, artistDb.Name!);

			if (getReleases)
			{
				// get saved releases in db for current artist
				artist.Releases = await _databaseReleasesController.GetReleasesDb(db, artistDb.Id!, getReleases);
			}

			artists.Add(artist);
		}*/

		return artists;
	}

	private async Task<SpotifyUserListUpdateArtists?> GetUpdateDb(IndexedDb db, string userId)
	{
		var updateDb = await _databaseUpdateController.Get(db, userId);

		if (updateDb?.Artists is null)
		{
			// TODO delete artists
			return null;
		}

		var update = new SpotifyUserListUpdateArtists(updateDb.Artists.Value)
		{
			LastUpdateAlbums = updateDb.ReleasesAlbums,
			LastUpdateTracks = updateDb.ReleasesTracks,
			LastUpdateAppears = updateDb.ReleasesAppears,
			LastUpdateCompilations = updateDb.ReleasesCompilations,
			LastUpdatePodcasts = updateDb.ReleasesPodcasts,
		};

		return update;
	}

	public async Task SaveArtists(string userId, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> artists)
	{
		// TODO remove unfollowed artists and deleted releases

		if (artists.Update is null)
		{
			// TODO
			throw new NullReferenceException(nameof(artists.Update));
		}
		if (artists.List is null)
		{
			// TODO
			throw new NullReferenceException(nameof(artists.List));
		}

		// create db
		var db = _dbController.GetDb();

		// update db
		await SaveUpdateDb(db, userId, artists.Update);

		// artists db
		await SaveArtistsDb(db, artists.List);

		// releases db
		await _databaseReleasesController.SaveReleasesDb(db, artists.List);

		// artist releases db
		await _databaseReleasesController.SaveArtistsReleasesDb(db, artists.List);
	}

	private async Task SaveUpdateDb(IndexedDb db, string userId, SpotifyUserListUpdateArtists update)
	{
		// TODO null
		//var updateDb = await _databaseUpdateController.Get(db, userId);

		var updateDb = await _databaseUpdateController.GetOrCreate(db, userId);

		/*if (updateDb is null)
		{
			// TODO
			throw new NullReferenceException(nameof(updateDb));
		}*/

		// update - update times

		updateDb.Artists = update.LastUpdateMain;

		updateDb.ReleasesAlbums = update.LastUpdateAlbums;
		updateDb.ReleasesAppears = update.LastUpdateAppears;
		updateDb.ReleasesCompilations = update.LastUpdateCompilations;
		updateDb.ReleasesPodcasts = update.LastUpdatePodcasts;
		updateDb.ReleasesTracks = update.LastUpdateTracks;

		Console.WriteLine("save update");
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUpdate);
		await table.StoreItemAsync(updateDb);
	}

	private async Task SaveArtistsDb(IndexedDb db, ISet<SpotifyArtist> artists)
	{
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyArtist);
		var newArtists = artists.Where(x => x.New);

		Console.WriteLine("save artists");
		foreach (var artist in newArtists)
		{
			var artistEntity = new SpotifyArtistEntity(artist, true);

			await table.StoreAsync(artistEntity);
		}
	}
}
