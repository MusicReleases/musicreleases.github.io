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

		// get db
		var loadingCategory = LoadingCategory.GetDb;
		_loaderService.StartLoading(loadingType, loadingCategory);
		var releasesDb = await _dbSpotifyArtistReleaseService.Get(artists, userId);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// get api
		loadingCategory = LoadingCategory.GetApi;
		_loaderService.StartLoading(loadingType, loadingCategory);
		var releases = await _spotifyReleaseService.GetReleases(releaseType, artists, releasesDb, forceUpdate);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// save db
		loadingCategory = LoadingCategory.SaveDb;
		_loaderService.StartLoading(loadingType, loadingCategory);
		await _dbSpotifyArtistReleaseService.Save(userId, releases);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// display
		Releases = releases;
		OnReleasesDataChanged?.Invoke();
	}
}
