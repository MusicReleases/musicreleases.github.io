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
	IDatabaseArtistReleasesController _databaseArtistReleasesController = databaseArtistReleasesController;

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> Get(string userId, bool getReleases)
	{
		using var db = await _dbFactory.Create<SpotifyReleasesDb>();

		if (db.Artists.Count < 1)
		{
			return null;
		}

		var artists = GetArtistsDb(db, userId, getReleases);

		var update = GetUpdateDb(db, userId);
		if (update is null)
		{
			return null;
		}

		var artistUpdate = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>(artists, update);

		return artistUpdate;
	}

	private ISet<SpotifyArtist> GetArtistsDb(SpotifyReleasesDb db, string userId, bool getReleases)
	{
		var artists = new SortedSet<SpotifyArtist>();

		foreach (var artistEntity in db.Artists.Where(x => x.Following))
		{
			if (artistEntity.Id.IsNullOrEmpty() || artistEntity.Name.IsNullOrEmpty())
			{
				continue;
			}

			var artist = new SpotifyArtist(artistEntity.Id, artistEntity.Name);

			if (getReleases)
			{
				artist.Releases = _databaseArtistReleasesController.GetReleasesDb(db, artistEntity.Id);
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

		SaveUpdateDb(db, userId, artists.Update);

		// update - lists
		var newArtists = SaveArtistsDb(artists.List);
		foreach (var newArtist in newArtists)
		{
			db.Artists.Add(newArtist);
		}

		var newReleases = _databaseArtistReleasesController.SaveReleasesDb(artists.List);
		foreach (var newRelease in newReleases)
		{
			db.Releases.Add(newRelease);
		}

		var artistsAndReleases = _databaseArtistReleasesController.SaveArtistsReleasesDb(artists.List);

		var newArtistsNotFollowed = artistsAndReleases.artists;
		foreach (var newArtistNotFollowed in newArtistsNotFollowed)
		{
			db.Artists.Add(newArtistNotFollowed);
		}

		var newArtistsReleases = artistsAndReleases.artistReleases;
		foreach (var newArtistsRelease in newArtistsReleases)
		{
			db.ArtistsReleases.Add(newArtistsRelease);
		}

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

	private ISet<SpotifyArtistEntity> SaveArtistsDb(ISet<SpotifyArtist> artists)
	{
		var newArtists = artists.Where(x => x.New);
		var artistsEntity = new HashSet<SpotifyArtistEntity>();

		foreach (var artist in newArtists)
		{
			var artistEntity = new SpotifyArtistEntity(artist, true);

			artistsEntity.Add(artistEntity);
		}

		return artistsEntity;
	}
}
