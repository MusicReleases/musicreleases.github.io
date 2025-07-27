using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyReleasesService(ISpotifyReleaseService spotifyReleaseService, ISpotifyUserService spotifyUserService, IDbSpotifyArtistReleaseService dbSpotifyArtistReleaseService, ILoaderService loaderService) : ISpotifyReleasesService
{
	private readonly ISpotifyReleaseService _spotifyReleaseService = spotifyReleaseService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;
	private readonly IDbSpotifyArtistReleaseService _dbSpotifyArtistReleaseService = dbSpotifyArtistReleaseService;
	private readonly ILoaderService _loaderService = loaderService;

	public SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? Releases { get; private set; } = null;

	public event Action? OnReleasesDataChanged;

	public async Task Get(ReleaseType releaseType, ISet<SpotifyArtist> artists, bool forceUpdate)
	{
		if (artists is null)
		{
			// provided artists
			// get from storage???
			return;
		}
		var userId = _spotifyUserService.GetUserIdRequired();
		var loadingType = LoadingType.Releases;
		var loadingCategoryGetDb = LoadingCategory.GetDb;
		var loadingCategoryGetApi = LoadingCategory.GetApi;
		var loadingCategorySaveDb = LoadingCategory.SaveDb;

		// get db
		_loaderService.StartLoading(loadingType, loadingCategoryGetDb);
		var releasesDb = Releases ?? await _dbSpotifyArtistReleaseService.Get(artists, userId);

		_loaderService.StartLoading(loadingType, loadingCategoryGetApi);
		_loaderService.StopLoading(loadingType, loadingCategoryGetDb);

		// get api
		var releasesApi = await _spotifyReleaseService.GetReleases(releaseType, artists, releasesDb, forceUpdate);

		if (releasesApi is not null)
		{
			_loaderService.StartLoading(loadingType, loadingCategorySaveDb);
		}
		_loaderService.StopLoading(loadingType, loadingCategoryGetApi);

		// save db
		if (releasesApi is not null)
		{
			await _dbSpotifyArtistReleaseService.Save(userId, releasesApi);
			_loaderService.StopLoading(loadingType, loadingCategorySaveDb);
		}

		// display
		Releases = releasesApi ?? releasesDb;
		OnReleasesDataChanged?.Invoke();
	}
}
