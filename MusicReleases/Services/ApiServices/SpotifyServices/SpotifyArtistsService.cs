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

		// get db
		var loadingCategory = LoadingCategory.GetDb;
		_loaderService.StartLoading(loadingType, loadingCategory);
		var artistsDb = await _dbUserArtistService.Get(userId);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// get api
		loadingCategory = LoadingCategory.GetApi;
		_loaderService.StartLoading(loadingType, loadingCategory);
		var artists = await _spotifyArtistService.GetUserFollowedArtists(artistsDb, forceUpdate);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// save db
		loadingCategory = LoadingCategory.SaveDb;
		_loaderService.StartLoading(loadingType, loadingCategory);
		await _dbUserArtistService.Save(userId, artists);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// display
		Artists = artists;
		OnArtistsDataChanged?.Invoke();
	}
}
