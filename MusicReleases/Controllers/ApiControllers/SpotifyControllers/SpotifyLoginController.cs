using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyLoginController(ISpotifyControllerUser spotifyControllerUser, NavigationManager navManager, ISpotifyLoginStorageController spotifyLoginStorageController) : ISpotifyLoginController
{
	private readonly ISpotifyControllerUser _spotifyControllerUser = spotifyControllerUser;
	private readonly ISpotifyLoginStorageController _spotifyLoginStorageController = spotifyLoginStorageController;
	private readonly NavigationManager _navManager = navManager;

	public ServiceType GetServiceType()
	{
		return ServiceType.Spotify;
	}

	public async Task<bool> IsUserSaved()
	{
		var user = await _spotifyLoginStorageController.GetSavedUser();

		return user is not null;
	}

	public async Task LoginUser()
	{
		// get user from local storage
		var user = await _spotifyLoginStorageController.GetSavedUser();

		if (user is not null)
		{
			var localStorageUser = await SetUserFromStorage(user);
			if (localStorageUser)
			{
				var userLogged = _spotifyControllerUser.IsLoggedIn();

				if (userLogged)
				{
					// navigate to releases page
					// TODO change release type
					if (!localStorageUser)
					{
						await _spotifyLoginStorageController.SaveUser();
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

		(var loginUrl, var loginVerifier) = _spotifyControllerUser.GetLoginUrl(redirectUrl);

		await _spotifyLoginStorageController.SaveLoginVerifier(loginVerifier);
		_navManager.NavigateTo(loginUrl.AbsoluteUri);
	}

	public async Task SetUser(StringValues code)
	{
		var localStorageUser = await SetUserFromStorage();
		if (!localStorageUser)
		{
			await SetUserFromUrl(code);
		}

		var userLogged = _spotifyControllerUser.IsLoggedIn();

		if (userLogged)
		{
			// navigate to releases page
			// TODO change release type
			if (!localStorageUser)
			{
				await _spotifyLoginStorageController.SaveUser();
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
		// get user from local storage
		spotifyUser ??= await _spotifyLoginStorageController.GetSavedUser();

		if (string.IsNullOrEmpty(spotifyUser?.Credentials?.RefreshToken))
		{
			return false;
		}

		// update access token
		await _spotifyControllerUser.RefreshUser(spotifyUser);

		if (!_spotifyControllerUser.IsLoggedIn())
		{
			// failed to refresh token
			await LogoutUser();
			return false;
		}

		await _spotifyLoginStorageController.SaveUser();

		return true;
	}

	public bool IsUserLoggedIn()
	{
		var userLoggedIn = _spotifyControllerUser.IsLoggedIn();
		return userLoggedIn;
	}

	private async Task SetUserFromUrl(StringValues code)
	{
		if (_spotifyControllerUser.IsLoggedIn())
		{
			return;
		}

		// get token from url
		var baseUrl = _navManager.BaseUri;

		var loginVerifier = await _spotifyLoginStorageController.GetLoginVerifier();
		if (string.IsNullOrEmpty(loginVerifier))
		{
			throw new NullReferenceException(nameof(loginVerifier));
		}

		var codeString = code.ToString();
		var userLogged = await _spotifyControllerUser.LoginUser(codeString, loginVerifier, baseUrl + "login/spotify");
	}

	public async Task LogoutUser()
	{
		// TODO logout - stop loading data from api

		// remove user
		await _spotifyLoginStorageController.DeleteSavedUser();

		_navManager.NavigateTo(_navManager.BaseUri);
	}
}
