using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseArtistReleasesController : IDatabaseArtistReleasesController
{
	public SortedSet<SpotifyRelease> GetReleasesDb(SpotifyReleasesDb db, string artistId)
	{
		var artistReleasesDb = db.ArtistsReleases
				.Where(x => x.ArtistId == artistId)
				.Select(x => db.Releases.FirstOrDefault(y => y.Id == x.ReleaseId))
				.Where(x => x is not null);

		var releases = new SortedSet<SpotifyRelease>();

		foreach (var releaseDb in artistReleasesDb)
		{
			if (releaseDb!.Id.IsNullOrEmpty() || releaseDb.Name.IsNullOrEmpty() || releaseDb.UrlApp.IsNullOrEmpty() || releaseDb.UrlWeb.IsNullOrEmpty() || releaseDb.UrlImage.IsNullOrEmpty())
			{
				continue;
			}

			var release = new SpotifyRelease
			{
				Id = releaseDb.Id,
				Name = releaseDb.Name,
				ReleaseDate = releaseDb.ReleaseDate,
				TotalTracks = releaseDb.TotalTracks,
				UrlApp = releaseDb.UrlApp,
				UrlWeb = releaseDb.UrlWeb,
				UrlImage = releaseDb.UrlImage,
				ReleaseType = releaseDb.ReleaseType,
				Artists = GetReleaseArtists(db, releaseDb.Id),
			};

			releases.Add(release);
		}

		return releases;
	}

	private HashSet<SpotifyArtist> GetReleaseArtists(SpotifyReleasesDb db, string releaseId)
	{
		var artists = db.ArtistsReleases
			.Where(x => x.ReleaseId == releaseId)
			.Join(db.Artists, releases => releases.ArtistId, artists => artists.Id, (r, a) => a)
			.Where(x => x.Id.IsNotNullOrEmpty() && x.Name.IsNotNullOrEmpty())
			.Select(x => new SpotifyArtist
			{
				Id = x.Id!,
				Name = x.Name!,
			})
			.ToHashSet();

		return artists;
	}

	public (ISet<SpotifyArtistReleaseEntity> artistReleases, ISet<SpotifyArtistEntity> artists) SaveArtistsReleasesDb(ISet<SpotifyArtist> artists)
	{
		var artistsReleasesEntity = new HashSet<SpotifyArtistReleaseEntity>();
		var artistsEntity = new HashSet<SpotifyArtistEntity>();

		// TODO save artists when release have more artists
		foreach (var artist in artists.Where(x => x.Releases is not null))
		{
			foreach (var release in artist.Releases!.Where(x => x.New))
			{
				var artistReleaseEntity = new SpotifyArtistReleaseEntity(artist.Id, release.Id);

				// save featuring artists
				var artistsDbIds = artists.Select(x => x.Id).ToHashSet();
				var notFollowedArtists = release.Artists.Where(x => !artistsDbIds.Contains(x.Id)).ToHashSet();

				//var notFollowedArtists = release.Artists.Where(x => !artists.Any(y => y.Id == x.Id));


				foreach (var notFollowedArtist in notFollowedArtists)
				{
					var notFollowedArtistReleasesDb = new SpotifyArtistReleaseEntity(notFollowedArtist.Id, release.Id);
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


	public ISet<SpotifyReleaseEntity> SaveReleasesDb(ISet<SpotifyArtist> artists)
	{
		var releasesEntity = artists
			.Where(x => x.Releases is not null)
			.SelectMany(x => x.Releases!)
			.Where(x => x.New)
			.Select(x => new SpotifyReleaseEntity(x))
			.ToHashSet();

		return releasesEntity;
	}
}
