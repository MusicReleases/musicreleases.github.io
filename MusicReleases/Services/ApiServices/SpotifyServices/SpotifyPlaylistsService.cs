using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyPlaylistsService(ISpotifyPlaylistService spotifyPlaylistService, ISpotifyUserService spotifyUserService, IDbSpotifyUserPlaylistService dbSpotifyUserPlaylistService, ILoaderService loaderService) : ISpotifyPlaylistsService
{
	private readonly ISpotifyPlaylistService _spotifyPlaylistService = spotifyPlaylistService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;
	private readonly IDbSpotifyUserPlaylistService _dbSpotifyUserPlaylistService = dbSpotifyUserPlaylistService;
	private readonly ILoaderService _loaderService = loaderService;

	public SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists { get; private set; } = null;

	public event Action? OnPlaylistsDataChanged;

	public async Task Get(bool forceUpdate)
	{
		var userId = _spotifyUserService.GetUserIdRequired();
		var loadingType = LoadingType.Playlists;
		var loadingCategoryGetDb = LoadingCategory.GetDb;
		var loadingCategoryGetApi = LoadingCategory.GetApi;
		var loadingCategorySaveDb = LoadingCategory.SaveDb;

		// get db
		_loaderService.StartLoading(loadingType, loadingCategoryGetDb);
		var playlistsDb = Playlists ?? await _dbSpotifyUserPlaylistService.Get(userId);

		_loaderService.StartLoading(loadingType, loadingCategoryGetApi);
		_loaderService.StopLoading(loadingType, loadingCategoryGetDb);

		// get api
		Console.WriteLine("api: get playlists - start");
		var playlistsApi = await _spotifyPlaylistService.GetUserPlaylists(true, playlistsDb, forceUpdate);
		Console.WriteLine("api: get playlists - end");

		if (playlistsApi is not null)
		{
			_loaderService.StartLoading(loadingType, loadingCategorySaveDb);
		}
		_loaderService.StopLoading(loadingType, loadingCategoryGetApi);

		// save db
		if (playlistsApi is not null)
		{
			await _dbSpotifyUserPlaylistService.Save(userId, playlistsApi);
			_loaderService.StopLoading(loadingType, loadingCategorySaveDb);
		}

		// display
		Playlists = playlistsApi ?? playlistsDb;
		OnPlaylistsDataChanged?.Invoke();
	}
}
