using JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;
using JakubKastner.SpotifyApi.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseSpotifyReleaseController(IDatabaseSpotifyController dbController) : IDatabaseSpotifyReleaseController
{
	private readonly IndexedDbStore _dbTable = dbController.GetTable(DbStorageTablesSpotify.SpotifyRelease);

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
		var releasesDb = _dbTable.GetAllAsync<SpotifyReleaseEntity>();

		var releases = new HashSet<SpotifyRelease>();

		await foreach (var releaseDb in releasesDb)
		{
			var releaseIdArtists = releaseIdsArtists.FirstOrDefault(x => x.ReleaseId == releaseDb.Id);
			if (releaseIdArtists is null)
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
				Artists = [.. releaseIdArtists.Artists],
			};

			releases.Add(release);
		}

		return releases;
	}

	public async Task Save(ISet<SpotifyRelease> releases)
	{
		foreach (var release in releases)
		{
			var releaseEntity = new SpotifyReleaseEntity(release);
			await _dbTable.StoreAsync(releaseEntity);
		}
	}

	public async Task Delete(string releaseId)
	{
		await _dbTable.RemoveItemAsync(releaseId);
	}
}
