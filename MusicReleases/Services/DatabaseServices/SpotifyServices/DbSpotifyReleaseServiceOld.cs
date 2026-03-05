using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyReleaseServiceOld(IDbSpotifyServiceOld dbService) : IDbSpotifyReleaseServiceOld
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyRelease);

	public async Task<ISet<SpotifyReleaseOld>?> Get(ISet<SpotifyReleaseArtistsDbObject> releaseIdsArtists)
	{
		// artists
		var releases = await GetDb(releaseIdsArtists);
		if (releases.Count < 1)
		{
			return null;
		}
		return releases;
	}

	private async Task<ISet<SpotifyReleaseOld>> GetDb(ISet<SpotifyReleaseArtistsDbObject> releaseIdsArtists)
	{
		Console.WriteLine("db: get releases - start");

		var releasesDb = _dbTable.GetAllAsync<SpotifyReleaseEntityOld>();

		var releases = new HashSet<SpotifyReleaseOld>();

		var releaseIdsArtistsDict = releaseIdsArtists.ToDictionary(x => x.ReleaseId);

		await foreach (var releaseDb in releasesDb)
		{
			if (!releaseIdsArtistsDict.TryGetValue(releaseDb.Id, out var releaseIdArtist))
			{
				continue;
			}

			var release = new SpotifyReleaseOld()
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

	public async Task Save(ISet<SpotifyReleaseOld> releases)
	{
		Console.WriteLine("db: save releases - start");

		foreach (var release in releases)
		{
			var releaseEntity = new SpotifyReleaseEntityOld(release);
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
