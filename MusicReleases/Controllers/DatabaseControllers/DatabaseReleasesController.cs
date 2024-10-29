using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseReleasesController
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



	public void SaveReleasesDb(SpotifyReleasesDb db, ISet<SpotifyArtist> artists)
	{
		var artistsWithReleases = artists.Where(x => x.Releases is not null);
		var releases = artistsWithReleases.SelectMany(x => x.Releases!);
		var newReleases = releases.Where(x => x.New);

		foreach (var newRelease in newReleases)
		{
			var releaseEntity = new SpotifyReleaseEntity(newRelease);
			db.Releases.Add(releaseEntity);
		}
	}













	public void SaveArtistsReleasesDb(SpotifyReleasesDb db, ISet<SpotifyArtist> artists)
	{
		// TODO save artists when release have more artists whitch are not followed
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
					db.ArtistsReleases.Add(notFollowedArtistReleasesDb);

					if (db.Artists.Any(x => x.Id == notFollowedArtist.Id))
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
					db.Artists.Add(notFollowedArtistDb);
				}


				db.ArtistsReleases.Add(artistReleaseEntity);
			}
		}
	}
}
