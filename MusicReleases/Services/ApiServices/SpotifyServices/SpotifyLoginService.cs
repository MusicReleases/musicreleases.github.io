using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.MusicReleases.Spotify.User;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

internal class SpotifyLoginService(SpotifyConfig spotifyConfig, ISpotifyUserClient spotifyUserClient, NavigationManager navManager, ISpotifyLoginStorageService spotifyLoginStorageService, ISpotifyUserDbService databaseUserService, ISpotifyUserUpdateDbService databaseUpdateService, ISpotifyReleaseFilterUrlSynchronizer releaseFilterUrlSynchronizer, ISettingsService settingsService, IBackgroundTaskManagerService backgroundTaskManagerService) : ISpotifyLoginService
{
	private readonly SpotifyConfig _spotifyConfig = spotifyConfig;
	private readonly ISpotifyUserClient _spotifyUserClient = spotifyUserClient;
	private readonly ISpotifyLoginStorageService _spotifyLoginStorageService = spotifyLoginStorageService;
	private readonly NavigationManager _navManager = navManager;
	private readonly ISpotifyUserDbService _databaseUserService = databaseUserService;
	private readonly ISpotifyUserUpdateDbService _databaseUpdateService = databaseUpdateService;
	private readonly ISpotifyReleaseFilterUrlSynchronizer _releaseFilterUrlSynchronizer = releaseFilterUrlSynchronizer;
	private readonly ISettingsService _settingsService = settingsService;
	private readonly IBackgroundTaskManagerService _backgroundTaskManagerService = backgroundTaskManagerService;

	public ServiceType GetServiceType()
	{
		return ServiceType.Spotify;
	}

	public async Task<bool> IsUserSaved()
	{
		var userDb = await GetUserFromDatabase();
		return userDb is not null;
	}

	public bool IsUserLoggedIn()
	{
		return _spotifyUserClient.IsLoggedIn();
	}

	public async Task LoginUser()
	{
		// get user from db
		var user = await GetUserFromDatabase();

		if (user is not null)
		{
			var localStorageUser = await SetUserFromDb(user);
			if (localStorageUser)
			{
				var userLogged = IsUserLoggedIn();

				if (userLogged)
				{
					// navigate to releases page
					if (!localStorageUser)
					{
						await SaveUser();
					}

					await _releaseFilterUrlSynchronizer.SetInitFilter();
				}
				else
				{
					// user is not logged in (error)
					_navManager.NavigateTo("");
				}
				return;
			}
		}

		// user is not saved in storage
		// spotify login
		var redirectUrl = _navManager.ToAbsoluteUri(_navManager.BaseUri + "login/spotify");
		var clientId = _spotifyConfig.ClientId;

		(var loginUrl, var loginVerifier) = _spotifyUserClient.GetLoginUrl(clientId, redirectUrl);

		await _spotifyLoginStorageService.SaveLoginVerifier(loginVerifier);
		_navManager.NavigateTo(loginUrl.AbsoluteUri);
	}

	public async Task SetUser(StringValues code)
	{
		var localStorageUser = await SetUserFromDb();
		if (!localStorageUser)
		{
			await SetUserFromUrl(code);
		}

		var userLogged = _spotifyUserClient.IsLoggedIn();

		if (!userLogged)
		{
			// user is not logged in (error)
			_navManager.NavigateTo("");
			return;
		}

		if (!localStorageUser)
		{
			await SaveUser();
		}

		// navigate to releases page
		await _releaseFilterUrlSynchronizer.SetInitFilter();
	}


	private async Task<bool> SetUserFromDb(SpotifyUser? spotifyUser = null)
	{
		// get user from database
		spotifyUser ??= await GetUserFromDatabase();

		if (spotifyUser is null)
		{
			return false;
		}

		// update access token
		await _spotifyUserClient.SetUserFromDb(spotifyUser);

		if (!_spotifyUserClient.IsLoggedIn())
		{
			// failed to refresh token
			await LogoutUser();
			return false;
		}

		// load settings from db
		await _settingsService.Initialize();

		await SaveUser();

		return true;
	}

	private async Task SetUserFromUrl(StringValues code)
	{
		if (_spotifyUserClient.IsLoggedIn())
		{
			return;
		}

		// get token from url
		var baseUrl = _navManager.BaseUri;

		var loginVerifier = await _spotifyLoginStorageService.GetLoginVerifier();

		loginVerifier = loginVerifier.Require();

		var codeString = code.ToString();
		var clientId = _spotifyConfig.ClientId;
		var userLogged = await _spotifyUserClient.LoginUser(clientId, codeString, loginVerifier, baseUrl + "login/spotify");

		if (userLogged)
		{
			// load settings from db
			await _settingsService.Initialize();
		}
	}

	public async Task LogoutUser()
	{
		// TODO logout - stop loading data from api
		// TODO logout - delete all user dbs

		// cancel tasks
		_backgroundTaskManagerService.CancelAllTasks();

		// remove user
		var user = _spotifyUserClient.GetUser();
		if (user is null)
		{
			return;
		}

		await _spotifyLoginStorageService.DeleteSavedUser();
		await _databaseUserService.Delete(user.Info.Id);
		await _databaseUpdateService.Delete(user.Info.Id, SpotifyDbUpdateType.User);

		_navManager.NavigateTo(_navManager.BaseUri);
	}

	private async Task SaveUser()
	{
		var user = _spotifyUserClient.GetUser();
		if (user is null)
		{
			return;
		}

		await _spotifyLoginStorageService.SaveUserId(user.Info.Id);
		await _databaseUserService.Save(user);
		await _databaseUpdateService.Save(user.Info.Id, SpotifyDbUpdateType.User);
	}

	private async Task<SpotifyUser?> GetUserFromDatabase()
	{
		var userId = await _spotifyLoginStorageService.GetSavedUserId();
		if (userId.IsNullOrEmpty())
		{
			return null;
		}

		var lastUpdate = await _databaseUpdateService.Get(userId, SpotifyDbUpdateType.User);

		var user = await _databaseUserService.Get(userId, lastUpdate);
		return user;
	}
}
