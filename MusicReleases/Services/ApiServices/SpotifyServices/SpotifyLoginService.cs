using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.MusicReleases.Spotify.Settings;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.MusicReleases.Spotify.User;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Base.Objects;
using JakubKastner.SpotifyApi.User;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

internal class SpotifyLoginService
(
	SpotifyConfig spotifyConfig,
	ISpotifyUserClient spotifyUserClient,
	ISpotifyLoginStorageService spotifyLoginStorageService,
	ISpotifyUserDbService databaseUserService,
	ISpotifyUserUpdateDbService databaseUpdateService,
	ISpotifyReleaseFilterUrlSynchronizer releaseFilterUrlSynchronizer,
	ISpotifySettingsService settingsService,
	IBackgroundTaskManagerService backgroundTaskManagerService,
	INavigationService navService
)
: ISpotifyLoginService
{
	private readonly SpotifyConfig _spotifyConfig = spotifyConfig;
	private readonly ISpotifyUserClient _spotifyUserClient = spotifyUserClient;
	private readonly ISpotifyLoginStorageService _spotifyLoginStorageService = spotifyLoginStorageService;
	private readonly ISpotifyUserDbService _databaseUserService = databaseUserService;
	private readonly ISpotifyUserUpdateDbService _databaseUpdateService = databaseUpdateService;
	private readonly ISpotifyReleaseFilterUrlSynchronizer _releaseFilterUrlSynchronizer = releaseFilterUrlSynchronizer;
	private readonly ISpotifySettingsService _settingsService = settingsService;
	private readonly IBackgroundTaskManagerService _backgroundTaskManagerService = backgroundTaskManagerService;
	private readonly INavigationService _navService = navService;

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
					_navService.Navigate("/", true, true, "user not logged in");
				}
				return;
			}
		}

		// user is not saved in storage
		// spotify login

		var redirectUrl = $"{_navService.BaseUrl}login/spotify";
		var clientId = _spotifyConfig.ClientId;

		(var loginUrl, var loginVerifier) = _spotifyUserClient.GetLoginUrl(clientId, redirectUrl);

		await _spotifyLoginStorageService.SaveLoginVerifier(loginVerifier);

		_navService.Navigate(loginUrl.AbsoluteUri, false, true, "user login (new)");
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
			_navService.Navigate("/", true, true, "user is not logged in (error)");
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
		var baseUrl = _navService.BaseUrl;

		var loginVerifier = await _spotifyLoginStorageService.GetLoginVerifier();

		loginVerifier = loginVerifier.Require();

		var codeString = code.ToString();
		var clientId = _spotifyConfig.ClientId;
		var redirectUrl = $"{baseUrl}login/spotify";

		var userLogged = await _spotifyUserClient.LoginUser(clientId, codeString, loginVerifier, redirectUrl);

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

		_navService.Navigate("/", true, false, "user logged out");
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
