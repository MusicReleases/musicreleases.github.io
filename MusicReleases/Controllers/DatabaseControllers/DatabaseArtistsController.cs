using IndexedDB.Blazor;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseArtistsController(IIndexedDbFactory dbFactory, IDatabaseUpdateController databaseUpdateController, ISpotifyControllerUser spotifyControllerUser, IDatabaseArtistReleasesController databaseArtistReleasesController) : IDatabaseArtistsController
{
	private readonly IIndexedDbFactory _dbFactory = dbFactory;
	private readonly IDatabaseUpdateController _databaseUpdateController = databaseUpdateController;
	private readonly ISpotifyControllerUser _spotifyControllerUser = spotifyControllerUser;
	private readonly IDatabaseArtistReleasesController _databaseArtistReleasesController = databaseArtistReleasesController;

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> GetFollowed(string userId, bool getReleases)
	{
		// create db
		using var db = await _dbFactory.Create<SpotifyReleasesDb>();

		// artists
		var artists = GetFollowedDb(db, userId, getReleases);
		if (artists.Count < 1)
		{
			return null;
		}

		// update
		var update = GetUpdateDb(db, userId);
		if (update is null)
		{
			return null;
		}

		var artistUpdate = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>(artists, update);

		return artistUpdate;
	}

	private ISet<SpotifyArtist> GetFollowedDb(SpotifyReleasesDb db, string userId, bool getReleases)
	{
		// get artists from db
		var followedArtistsDb = db.Artists.Where(x => x.Following);
		var artists = new SortedSet<SpotifyArtist>();

		foreach (var artistDb in followedArtistsDb)
		{
			if (artistDb.Id.IsNullOrEmpty() || artistDb.Name.IsNullOrEmpty())
			{
				continue;
			}

			var artist = new SpotifyArtist(artistDb.Id, artistDb.Name);

			if (getReleases)
			{
				// get saved releases in db for current artist
				artist.Releases = _databaseArtistReleasesController.GetReleasesDb(db, artistDb.Id);
			}

			artists.Add(artist);
		}
		return artists;
	}

	private SpotifyUserListUpdateArtists? GetUpdateDb(SpotifyReleasesDb db, string userId)
	{
		var updateDb = _databaseUpdateController.Get(db, userId);

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

		using var db = await _dbFactory.Create<SpotifyReleasesDb>();

		// update db
		SaveUpdateDb(db, userId, artists.Update);

		// artists db
		SaveArtistsDb(db, artists.List);

		// releases db
		_databaseArtistReleasesController.SaveReleasesDb(db, artists.List);

		// artist releases db
		_databaseArtistReleasesController.SaveArtistsReleasesDb(db, artists.List);

		await db.SaveChanges();
	}

	private void SaveUpdateDb(SpotifyReleasesDb db, string userId, SpotifyUserListUpdateArtists update)
	{
		var updateDb = _databaseUpdateController.Get(db, userId);
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
	}

	private void SaveArtistsDb(SpotifyReleasesDb db, ISet<SpotifyArtist> artists)
	{
		var newArtists = artists.Where(x => x.New);

		foreach (var artist in newArtists)
		{
			var artistEntity = new SpotifyArtistEntity(artist, true);

			db.Artists.Add(artistEntity);
		}
	}
}
