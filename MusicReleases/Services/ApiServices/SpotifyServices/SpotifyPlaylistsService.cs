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

		// get db
		var loadingCategory = LoadingCategory.GetDb;
		_loaderService.StartLoading(loadingType, loadingCategory);
		var playlistsDb = await _dbSpotifyUserPlaylistService.Get(userId);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// get api
		loadingCategory = LoadingCategory.GetApi;
		_loaderService.StartLoading(loadingType, loadingCategory);
		var playlists = await _spotifyPlaylistService.GetUserPlaylists(true, playlistsDb, forceUpdate);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// save db
		loadingCategory = LoadingCategory.SaveDb;
		_loaderService.StartLoading(loadingType, loadingCategory);
		await _dbSpotifyUserPlaylistService.Save(userId, playlists);
		_loaderService.StopLoading(loadingType, loadingCategory);

		// display
		Playlists = playlists;
		OnPlaylistsDataChanged?.Invoke();
	}
}
