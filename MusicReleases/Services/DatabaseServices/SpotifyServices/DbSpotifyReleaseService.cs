using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyReleaseService(IDbSpotifyServiceOld dbService) : IDbSpotifyReleaseService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyRelease);

	public async Task<ISet<SpotifyRelease>?> Get(ISet<SpotifyReleaseArtistsDbObject> releaseIdsArtists)
	{
		// artists
		var releases = await GetDb(releaseIdsArtists);
		if (releases.Count < 1)
		{
			return null;
		}
		return releases;
	}

	private async Task<ISet<SpotifyRelease>> GetDb(ISet<SpotifyReleaseArtistsDbObject> releaseIdsArtists)
	{
		Console.WriteLine("db: get releases - start");

		var releasesDb = _dbTable.GetAllAsync<SpotifyReleaseEntity>();

		var releases = new HashSet<SpotifyRelease>();

		var releaseIdsArtistsDict = releaseIdsArtists.ToDictionary(x => x.ReleaseId);

		await foreach (var releaseDb in releasesDb)
		{
			if (!releaseIdsArtistsDict.TryGetValue(releaseDb.Id, out var releaseIdArtist))
			{
				continue;
			}

			var release = new SpotifyRelease()
			{
				Id = releaseDb.Id,
				Name = releaseDb.Name,
				ReleaseDate = releaseDb.ReleaseDate,
				TotalTracks = releaseDb.TotalTracks,
				UrlApp = releaseDb.UrlApp,
				UrlWeb = releaseDb.UrlWeb,
				UrlImage = releaseDb.UrlImage,
				ReleaseType = releaseDb.ReleaseType,
				Artists = [.. releaseIdArtist.Artists],
			};

			releases.Add(release);
		}
		Console.WriteLine("db: get releases - end");

		return releases;
	}

	public async Task Save(ISet<SpotifyRelease> releases)
	{
		Console.WriteLine("db: save releases - start");

		foreach (var release in releases)
		{
			var releaseEntity = new SpotifyReleaseEntity(release);
			await _dbTable.StoreAsync(releaseEntity);
		}
		Console.WriteLine("db: save releases - end");
	}

	public async Task Delete(string releaseId)
	{
		Console.WriteLine("db: delete release - start");
		await _dbTable.RemoveItemAsync(releaseId);
		Console.WriteLine("db: delete release - end");
	}
}
