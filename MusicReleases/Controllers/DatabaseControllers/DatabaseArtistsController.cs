using IndexedDB.Blazor;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseArtistsController(IIndexedDbFactory dbFactory, IDatabaseUpdateController databaseUpdateController, ISpotifyControllerUser spotifyControllerUser) : IDatabaseArtistsController
{
	private readonly IIndexedDbFactory _dbFactory = dbFactory;
	private readonly IDatabaseUpdateController _databaseUpdateController = databaseUpdateController;
	private readonly ISpotifyControllerUser _spotifyControllerUser = spotifyControllerUser;

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> GetArtists()
	{
		using (var db = await _dbFactory.Create<SpotifyReleasesDb>())
		{
			return await GetArtistsDb(db);
		}
	}

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> GetArtistsDb(SpotifyReleasesDb db)
	{
		if (db.Artists is null || db.Artists.Count < 1)
		{
			return null;
		}

		var artists = new SortedSet<SpotifyArtist>();

		foreach (var artistEntity in db.Artists.Where(x => x.Following))
		{
			var artist = new SpotifyArtist()
			{
				Id = artistEntity.Id,
				Name = artistEntity.Name,
				Releases = GetReleasesDb(artistEntity.Id, db),
			};

			artists.Add(artist);
		}

		// TODO
		var user = _spotifyControllerUser.GetUserRequired();
		var updateDb = _databaseUpdateController.Get(user.Info.Id, db);

		if (updateDb?.Artists is null)
		{
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

		var artistUpdate = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>(artists, update);

		return artistUpdate;
	}

	private SortedSet<SpotifyRelease> GetReleasesDb(string artistId, SpotifyReleasesDb db)
	{
		var artistReleasesDb = db.ArtistsReleases.Where(x => x.ArtistId == artistId)
				.Select(x => db.Releases.FirstOrDefault(y => y.Id == x.ReleaseId))
				.Where(x => x is not null);

		var releases = new SortedSet<SpotifyRelease>();

		foreach (var releaseDb in artistReleasesDb)
		{
			var release = new SpotifyRelease()
			{
				Id = releaseDb!.Id,
				Name = releaseDb.Name,
				ReleaseDate = releaseDb.ReleaseDate,
				TotalTracks = releaseDb.TotalTracks,
				UrlApp = releaseDb.UrlApp,
				UrlWeb = releaseDb.UrlWeb,
				UrlImage = releaseDb.UrlImage,
				ReleaseType = releaseDb.ReleaseType,
				Artists = GetReleaseArtists(releaseDb.Id, db),
			};

			releases.Add(release);
		}

		return releases;
	}
	private HashSet<SpotifyArtist> GetReleaseArtists(string releaseId, SpotifyReleasesDb db)
	{
		var artists = db.ArtistsReleases.Where(x => x.ReleaseId == releaseId)
											.Join(db.Artists, releases => releases.ArtistId, artists => artists.Id, (r, a) => a)
											.Select(x => new SpotifyArtist()
											{
												Id = x.Id,
												Name = x.Name,
											})
											.ToHashSet();

		return artists;
	}



	public async Task SaveArtists(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> artists)
	{

		using (var db = await _dbFactory.Create<SpotifyReleasesDb>())
		{
			// TODO remove unfollowed artists and deleted releases

			// TODO
			var user = _spotifyControllerUser.GetUserRequired();
			var updateDb = _databaseUpdateController.Get(user.Info.Id, db);

			// update - update times
			updateDb!.Artists = artists.Update!.LastUpdateMain;

			updateDb.ReleasesAlbums = artists.Update.LastUpdateAlbums;
			updateDb.ReleasesAppears = artists.Update.LastUpdateAppears;
			updateDb.ReleasesCompilations = artists.Update.LastUpdateCompilations;
			updateDb.ReleasesPodcasts = artists.Update.LastUpdatePodcasts;
			updateDb.ReleasesTracks = artists.Update.LastUpdateTracks;


			// update - lists
			var newArtists = SaveArtistsDb(artists.List!);
			foreach (var newArtist in newArtists)
			{
				db.Artists.Add(newArtist);
			}

			var newReleases = SaveReleasesDb(artists.List!);
			foreach (var newRelease in newReleases)
			{
				db.Releases.Add(newRelease);
			}

			var artistsAndReleases = SaveArtistsReleasesDb(artists.List!);

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
	}

	private ISet<SpotifyArtistEntity> SaveArtistsDb(ISet<SpotifyArtist> artists)
	{
		var newArtists = artists.Where(x => x.New);
		var artistsEntity = new HashSet<SpotifyArtistEntity>();

		foreach (var artist in newArtists)
		{
			var artistEntity = new SpotifyArtistEntity()
			{
				Id = artist.Id,
				Name = artist.Name,
				Following = true,
			};

			artistsEntity.Add(artistEntity);
		}

		return artistsEntity;
	}

	private (ISet<SpotifyArtistReleaseEntity> artistReleases, ISet<SpotifyArtistEntity> artists) SaveArtistsReleasesDb(ISet<SpotifyArtist> artists)
	{
		var artistsReleasesEntity = new HashSet<SpotifyArtistReleaseEntity>();
		var artistsEntity = new HashSet<SpotifyArtistEntity>();

		// TODO save artists when release have more artists
		foreach (var artist in artists.Where(x => x.Releases is not null))
		{
			foreach (var release in artist.Releases!.Where(x => x.New))
			{
				var artistReleaseEntity = new SpotifyArtistReleaseEntity()
				{
					Id = Guid.NewGuid(),
					ArtistId = artist.Id,
					ReleaseId = release.Id,
				};

				// save featuring artists
				var artistsDbIds = artists.Select(x => x.Id).ToHashSet();
				var notFollowedArtists = release.Artists.Where(x => !artistsDbIds.Contains(x.Id)).ToList();

				//var notFollowedArtists = release.Artists.Where(x => !artists.Any(y => y.Id == x.Id));


				foreach (var notFollowedArtist in notFollowedArtists)
				{
					var notFollowedArtistReleasesDb = new SpotifyArtistReleaseEntity()
					{
						Id = Guid.NewGuid(),
						ArtistId = notFollowedArtist.Id,
						ReleaseId = release.Id,
					};
					artistsReleasesEntity.Add(notFollowedArtistReleasesDb);

					if (artistsEntity.Any(x => x.Id == notFollowedArtist.Id))
					{
						// artist allready added
						continue;
					}

					var notFollowedArtistDb = new SpotifyArtistEntity()
					{
						Id = notFollowedArtist.Id,
						Name = notFollowedArtist.Name,
						Following = false,
					};
					artistsEntity.Add(notFollowedArtistDb);
				}


				artistsReleasesEntity.Add(artistReleaseEntity);
			}
		}

		return (artistsReleasesEntity, artistsEntity);
	}

	private ISet<SpotifyReleaseEntity> SaveReleasesDb(ISet<SpotifyArtist> artists)
	{
		var releases = artists.Where(x => x.Releases is not null).SelectMany(x => x.Releases!);
		var newReleases = releases.Where(x => x.New);
		var releasesEntity = new HashSet<SpotifyReleaseEntity>();

		foreach (var release in newReleases)
		{
			var releaseEntity = new SpotifyReleaseEntity()
			{
				Id = release.Id,
				Name = release.Name,
				ReleaseDate = release.ReleaseDate,
				TotalTracks = release.TotalTracks,
				UrlApp = release.UrlApp,
				UrlWeb = release.UrlWeb,
				UrlImage = release.UrlImage,
				ReleaseType = release.ReleaseType,
			};

			releasesEntity.Add(releaseEntity);
		}

		return releasesEntity;
	}
}
