using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyArtistsService(ISpotifyArtistService spotifyArtistService, ISpotifyUserService spotifyUserService, IDbSpotifyUserArtistService dbUserArtistService, ILoaderService loaderService) : ISpotifyArtistsService
{
	private readonly ISpotifyArtistService _spotifyArtistService = spotifyArtistService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;
	private readonly IDbSpotifyUserArtistService _dbUserArtistService = dbUserArtistService;
	private readonly ILoaderService _loaderService = loaderService;

	public SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? Artists { get; private set; } = null;

	public event Action? OnArtistsDataChanged;

	public async Task Get(bool forceUpdate)
	{
		var userId = _spotifyUserService.GetUserIdRequired();
		var loadingType = LoadingType.Artists;
		var loadingCategoryGetDb = LoadingCategory.GetDb;
		var loadingCategoryGetApi = LoadingCategory.GetApi;
		var loadingCategorySaveDb = LoadingCategory.SaveDb;

		// get db
		_loaderService.StartLoading(loadingType, loadingCategoryGetDb);
		var artistsDb = Artists ?? await _dbUserArtistService.Get(userId);


		_loaderService.StartLoading(loadingType, loadingCategoryGetApi);
		_loaderService.StopLoading(loadingType, loadingCategoryGetDb);

		// get api
		Console.WriteLine("api: get artists - start");
		var artistsApi = await _spotifyArtistService.GetUserFollowedArtists(artistsDb, forceUpdate);
		Console.WriteLine("api: get artists - end");

		if (artistsApi is not null)
		{
			_loaderService.StartLoading(loadingType, loadingCategorySaveDb);
		}
		_loaderService.StopLoading(loadingType, loadingCategoryGetApi);

		// save db
		if (artistsApi is not null)
		{
			await _dbUserArtistService.Save(userId, artistsApi);
			_loaderService.StopLoading(loadingType, loadingCategorySaveDb);
		}

		// display
		Artists = artistsApi ?? artistsDb;
		OnArtistsDataChanged?.Invoke();
	}
}
