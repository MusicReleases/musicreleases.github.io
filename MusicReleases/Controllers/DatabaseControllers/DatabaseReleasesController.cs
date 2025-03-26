using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseReleasesController(IDatabaseController dbController) : IDatabaseReleasesController
{
	private readonly IDatabaseController _dbController = dbController;

	private readonly IndexedDbStore _dbTableRelease = dbController.GetTable(DbStorageTablesSpotify.SpotifyRelease);
	private readonly IndexedDbStore _dbTableArtistRelease = dbController.GetTable(DbStorageTablesSpotify.SpotifyArtistRelease);
	private readonly IndexedDbStore _dbTableArtist = dbController.GetTable(DbStorageTablesSpotify.SpotifyArtist);

	public async Task<SortedSet<SpotifyRelease>> GetReleasesDb(string artistId, bool getReleases)
	{
		// artist releases table
		Console.WriteLine("artist releases");
		var artistReleasesDb = _dbTableArtistRelease.GetAllAsync<SpotifyArtistReleaseEntity>();
		var releaseIds = new HashSet<string>();

		await foreach (var artistReleaseDb in artistReleasesDb)
		{
			if (artistReleaseDb.ArtistId != artistId)
			{
				continue;
			}
			releaseIds.Add(artistReleaseDb.ReleaseId);
		}
		/*var artistReleasesDb = await _dbController
			.GetTable(db, DbStorageTablesSpotify.ArtistsReleases)
			.Query<SpotifyArtistReleaseEntity>()
			.Where(x => x.ArtistId == artistId)
			.ToListAsync();

		var releaseIds = artistReleasesDb.Select(x => x.ReleaseId);*/


		// releases table
		Console.WriteLine("get releases");
		var releasesDb = _dbTableRelease.GetAllAsync<SpotifyReleaseEntity>();
		var releases = new SortedSet<SpotifyRelease>();

		await foreach (var releaseDb in releasesDb)
		{
			if (!releaseIds.Contains(releaseDb.Id))
			{
				continue;
			}

			var artist = new SpotifyRelease
			{
				Id = releaseDb.Id,
				Name = releaseDb.Name,
				ReleaseDate = releaseDb.ReleaseDate,
				TotalTracks = releaseDb.TotalTracks,
				UrlApp = releaseDb.UrlApp,
				UrlWeb = releaseDb.UrlWeb,
				UrlImage = releaseDb.UrlImage,
				ReleaseType = releaseDb.ReleaseType,
				Artists = await GetReleaseArtists(releaseDb.Id),
			};

			releases.Add(artist);
		}
		/*var releasesDb = await _dbController
			.GetTable(db, DbStorageTablesSpotify.Releases)
			.Query<SpotifyReleaseEntity>()
			.Where(x => releaseIds.Contains(x.Id))
			.ToListAsync();

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
		}*/
		return releases;
	}

	private async Task<HashSet<SpotifyArtist>> GetReleaseArtists(string releaseId)
	{

		// artist releases table
		Console.WriteLine("get artist releases");
		var artistReleasesDb = _dbTableArtistRelease.GetAllAsync<SpotifyArtistReleaseEntity>();
		var artistIds = new HashSet<string>();

		await foreach (var artistReleaseDb in artistReleasesDb)
		{
			if (artistReleaseDb.ReleaseId != releaseId)
			{
				continue;
			}
			artistIds.Add(artistReleaseDb.ArtistId);
		}
		/*var artistReleasesDb = await tableArtistReleases
			.Query<SpotifyArtistReleaseEntity>()
			.Where(x => x.ReleaseId == releaseId)
			.ToListAsync();
		var artistIds = artistReleasesDb.Select(x => x.ArtistId);*/

		// artists table
		Console.WriteLine("get artists");
		var artistsDb = _dbTableArtist.GetAllAsync<SpotifyArtistEntity>();
		var artists = new HashSet<SpotifyArtist>();

		await foreach (var artistDb in artistsDb)
		{
			if (artistDb.Id.IsNullOrEmpty() || !artistIds.Contains(artistDb.Id))
			{
				continue;
			}

			var artist = new SpotifyArtist
			{
				Id = artistDb.Id,
				Name = artistDb.Name!,
			};

			artists.Add(artist);
		}
		/*var artistsDb = await _dbController
			.GetTable(db, DbStorageTablesSpotify.Artists)
			.Query<SpotifyArtistEntity>()
			.Where(x => artistIds.Contains(x.Id))
			.ToListAsync();
		var artists = artistsDb
			.Select(x => new SpotifyArtist
			{
				Id = x.Id!,
				Name = x.Name!,
			})
			.ToHashSet();*/

		return artists;
	}

	public async Task SaveArtistsReleasesDb(ISet<SpotifyArtist> artists)
	{
		var artistsDb = _dbTableArtist.GetAllAsync<SpotifyArtist>();
		var artistsDbIds = new HashSet<string>();
		await foreach (var artistDb in artistsDb)
		{
			artistsDbIds.Add(artistDb.Id);
		}
		var artistsReleaseToSave = new HashSet<SpotifyArtistReleaseEntity>();
		var artistsToSave = new HashSet<SpotifyArtistEntity>();

		// TODO save artists when release have more artists
		foreach (var artist in artists.Where(x => x.Releases is not null))
		{
			foreach (var release in artist.Releases!.Where(x => x.New))
			{
				var artistReleaseEntity = new SpotifyArtistReleaseEntity(artist.Id, release.Id);

				// save featuring artists
				var artistsIds = artists.Select(x => x.Id).ToHashSet();
				var notFollowedArtists = release.Artists.Where(x => !artistsIds.Contains(x.Id)).ToHashSet();

				//var notFollowedArtists = release.Artists.Where(x => !artists.Any(y => y.Id == x.Id));


				foreach (var notFollowedArtist in notFollowedArtists)
				{
					var notFollowedArtistReleasesDb = new SpotifyArtistReleaseEntity(notFollowedArtist.Id, release.Id);
					Console.WriteLine("save artist release (not followed)");
					//artistsReleaseToSave.Add(notFollowedArtistReleasesDb);
					await _dbTableArtistRelease.StoreItemAsync(notFollowedArtistReleasesDb);

					//if (await tableArtists.Query<SpotifyArtist>().AnyAsync(x => x.Id == notFollowedArtist.Id))
					if (artistsDbIds.Any(x => x == notFollowedArtist.Id))
					{
						// artist allready added
						continue;
					}

					var notFollowedArtistDb = new SpotifyArtistEntity(notFollowedArtist, false);
					/*{
						Id = notFollowedArtist.Id,
						Name = notFollowedArtist.Name,
						Following = false,
					};*/
					//artistsToSave.Add(notFollowedArtistDb);
					Console.WriteLine("save not followed artist");
					await _dbTableArtist.StoreItemAsync(notFollowedArtistDb);
				}

				Console.WriteLine("save artist release (followed)");
				//artistsReleaseToSave.Add(artistReleaseEntity);
				await _dbTableArtistRelease.StoreItemAsync(artistReleaseEntity);
			}
		}

		/*var tableArtistsReleases = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyArtistRelease);
		foreach (var artistReleaseToSave in artistsReleaseToSave)
		{
			Console.WriteLine("save artist release");
			await tableArtistsReleases.StoreItemAsync(artistReleaseToSave);
		}
		//var tableArtists = _dbController.GetTable(db, DbStorageTablesSpotify.Artists);
		foreach (var artistToSave in artistsToSave)
		{
			Console.WriteLine("save not followed artist");
			await tableArtists.StoreItemAsync(artistToSave);
		}*/
	}

	public async Task SaveReleasesDb(ISet<SpotifyArtist> artists)
	{
		var artistsWithReleases = artists.Where(x => x.Releases is not null);
		var releases = artistsWithReleases.SelectMany(x => x.Releases!);
		var newReleases = releases.Where(x => x.New);

		Console.WriteLine("save releases");
		foreach (var newRelease in newReleases)
		{
			var releaseEntity = new SpotifyReleaseEntity(newRelease);
			await _dbTableRelease.StoreItemAsync(releaseEntity);
		}
	}
}
