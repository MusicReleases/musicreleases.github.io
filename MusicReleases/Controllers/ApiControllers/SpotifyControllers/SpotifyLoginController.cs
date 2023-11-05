using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyLoginController : ISpotifyLoginController
{
    private readonly ISpotifyControllerUser _spotifyControllerUser;
    private readonly ISpotifyLoginStorageController _spotifyLoginStorageController;
    private readonly NavigationManager _navManager;

    public SpotifyLoginController(ISpotifyControllerUser spotifyControllerUser, NavigationManager navManager, ISpotifyLoginStorageController spotifyLoginStorageController)
    {
        _spotifyControllerUser = spotifyControllerUser;
        _navManager = navManager;
        _spotifyLoginStorageController = spotifyLoginStorageController;
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
        var redirectUrl = _navManager.ToAbsoluteUri(_navManager.BaseUri + "login");

        (var loginUrl, var loginVerifier) = _spotifyControllerUser.GetLoginUrl(redirectUrl);

        await _spotifyLoginStorageController.SaveLoginVerifier(loginVerifier);
        _navManager.NavigateTo(loginUrl.AbsoluteUri);

        return;
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
        await _spotifyLoginStorageController.SaveUser();

        return true;
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
        var userLogged = await _spotifyControllerUser.LoginUser(codeString, loginVerifier, baseUrl + "login");
    }
}
