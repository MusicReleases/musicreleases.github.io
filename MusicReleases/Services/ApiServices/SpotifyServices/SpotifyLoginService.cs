using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyLoginService(ISpotifyUserService spotifyUserService, NavigationManager navManager, ISpotifyLoginStorageService spotifyLoginStorageService, IDbSpotifyUserService databaseUserService) : ISpotifyLoginService
{
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;
	private readonly ISpotifyLoginStorageService _spotifyLoginStorageService = spotifyLoginStorageService;
	private readonly NavigationManager _navManager = navManager;
	private readonly IDbSpotifyUserService _databaseUserService = databaseUserService;

	public ServiceType GetServiceType()
	{
		return ServiceType.Spotify;
	}

	public async Task<bool> IsUserSaved()
	{
		var user = await GetUserFromDatabase();

		return user is not null;
	}

	public async Task LoginUser()
	{
		// get user from db
		var user = await GetUserFromDatabase();

		if (user is not null)
		{
			var localStorageUser = await SetUserFromStorage(user);
			if (localStorageUser)
			{
				var userLogged = _spotifyUserService.IsLoggedIn();

				if (userLogged)
				{
					// navigate to releases page
					// TODO change release type
					if (!localStorageUser)
					{
						await SaveUser();
					}
					_navManager.NavigateTo("releases/albums");
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

		(var loginUrl, var loginVerifier) = _spotifyUserService.GetLoginUrl(redirectUrl);

		await _spotifyLoginStorageService.SaveLoginVerifier(loginVerifier);
		_navManager.NavigateTo(loginUrl.AbsoluteUri);
	}

	public async Task SetUser(StringValues code)
	{
		var localStorageUser = await SetUserFromStorage();
		if (!localStorageUser)
		{
			await SetUserFromUrl(code);
		}

		var userLogged = _spotifyUserService.IsLoggedIn();

		if (userLogged)
		{
			// navigate to releases page
			// TODO change release type
			if (!localStorageUser)
			{
				await SaveUser();
			}
			_navManager.NavigateTo("releases/albums");
		}
		else
		{
			// user is not logged in (error)
			_navManager.NavigateTo("");
		}
	}


	private async Task<bool> SetUserFromStorage(SpotifyUser? spotifyUser = null)
	{
		// get user from database
		spotifyUser ??= await GetUserFromDatabase();

		if (string.IsNullOrEmpty(spotifyUser?.Credentials?.RefreshToken))
		{
			return false;
		}

		// update access token
		await _spotifyUserService.RefreshUser(spotifyUser);

		if (!_spotifyUserService.IsLoggedIn())
		{
			// failed to refresh token
			await LogoutUser();
			return false;
		}

		await SaveUser();

		return true;
	}

	public bool IsUserLoggedIn()
	{
		var userLoggedIn = _spotifyUserService.IsLoggedIn();
		return userLoggedIn;
	}

	private async Task SetUserFromUrl(StringValues code)
	{
		if (_spotifyUserService.IsLoggedIn())
		{
			return;
		}

		// get token from url
		var baseUrl = _navManager.BaseUri;

		var loginVerifier = await _spotifyLoginStorageService.GetLoginVerifier();
		if (string.IsNullOrEmpty(loginVerifier))
		{
			throw new NullReferenceException(nameof(loginVerifier));
		}

		var codeString = code.ToString();
		var userLogged = await _spotifyUserService.LoginUser(codeString, loginVerifier, baseUrl + "login/spotify");
	}

	public async Task LogoutUser()
	{
		// TODO logout - stop loading data from api

		// remove user
		var user = _spotifyUserService.GetUser();
		if (user?.Info is null || user.Credentials is null)
		{
			return;
		}

		await _databaseUserService.DeleteAllUserDatabases(user.Info.Id);
		await _spotifyLoginStorageService.DeleteSavedUser();

		_navManager.NavigateTo(_navManager.BaseUri);
	}

	private async Task SaveUser()
	{
		var user = _spotifyUserService.GetUser();
		if (user?.Info is null || user.Credentials is null)
		{
			return;
		}

		await _spotifyLoginStorageService.SaveUserId(user.Info.Id);
		await _databaseUserService.Save(user);
	}

	private async Task<SpotifyUser?> GetUserFromDatabase()
	{
		var userId = await _spotifyLoginStorageService.GetSavedUserId();
		if (userId.IsNullOrEmpty())
		{
			return null;
		}

		var user = await _databaseUserService.Get(userId!);
		return user;
	}
}
