using JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseArtistsController(IDatabaseController dbController, IDatabaseUpdateController dbUpdateController, IDatabaseReleasesController dbReleasesController) : IDatabaseArtistsController
{
	private readonly IDatabaseUpdateController _dbUpdateController = dbUpdateController;
	private readonly IDatabaseReleasesController _dbReleasesController = dbReleasesController;

	private readonly IndexedDbStore _dbTable = dbController.GetTable(DbStorageTablesSpotify.SpotifyArtist);

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> GetFollowed(string userId, bool getReleases)
	{
		// artists
		var artists = await GetFollowedDb(userId, getReleases);
		if (artists.Count < 1)
		{
			return null;
		}

		// update
		var update = await GetUpdateDb(userId);
		if (update is null)
		{
			return null;
		}

		var artistUpdate = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>(artists, update);

		return artistUpdate;
	}

	private async Task<ISet<SpotifyArtist>> GetFollowedDb(string userId, bool getReleases)
	{
		// TODO user artist db table

		Console.WriteLine("get artists");
		// get artists from db
		var artistsDb = _dbTable.GetAllAsync<SpotifyArtistEntity>();
		var artists = new HashSet<SpotifyArtist>();

		await foreach (var artistDb in artistsDb)
		{
			if (artistDb.Following)
			{
				continue;
			}

			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name);

			if (getReleases)
			{
				// get saved releases in db for current artist
				artist.Releases = await _dbReleasesController.GetReleasesDb(artistDb.Id, getReleases);
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

	private async Task<SpotifyUserListUpdateArtists?> GetUpdateDb(string userId)
	{
		var updateDb = await _dbUpdateController.Get(userId);

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

		// update db
		await SaveUpdateDb(userId, artists.Update);

		// artists db
		await SaveArtistsDb(artists.List);

		// releases db
		await _dbReleasesController.SaveReleasesDb(artists.List);

		// artist releases db
		await _dbReleasesController.SaveArtistsReleasesDb(artists.List);
	}

	private async Task SaveUpdateDb(string userId, SpotifyUserListUpdateArtists update)
	{
		// TODO null
		//var updateDb = await _databaseUpdateController.Get(db, userId);

		var updateDb = await _dbUpdateController.Get(userId);

		if (updateDb is null)
		{
			// TODO
			throw new NullReferenceException(nameof(updateDb));
		}

		// update - update times

		updateDb.Artists = update.LastUpdateMain;

		updateDb.ReleasesAlbums = update.LastUpdateAlbums;
		updateDb.ReleasesAppears = update.LastUpdateAppears;
		updateDb.ReleasesCompilations = update.LastUpdateCompilations;
		updateDb.ReleasesPodcasts = update.LastUpdatePodcasts;
		updateDb.ReleasesTracks = update.LastUpdateTracks;

		await _dbUpdateController.Update(updateDb);
	}

	private async Task SaveArtistsDb(ISet<SpotifyArtist> artists)
	{
		var newArtists = artists.Where(x => x.New);

		Console.WriteLine("save artists");
		foreach (var artist in newArtists)
		{
			var artistEntity = new SpotifyArtistEntity(artist, true);

			await _dbTable.StoreAsync(artistEntity);
		}
	}
}
