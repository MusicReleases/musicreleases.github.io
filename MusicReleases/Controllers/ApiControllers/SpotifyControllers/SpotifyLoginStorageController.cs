using Blazored.LocalStorage;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyLoginStorageController : ISpotifyLoginStorageController
{
    private readonly ISpotifyControllerUser _spotifyControllerUser;
    private readonly ILocalStorageService _localStorage;

    private readonly string _localStorageKeyInfo;
    private readonly string _localStorageKeyCredentials;
    private readonly string _localStorageKeyVerifier;

    public SpotifyLoginStorageController(ISpotifyControllerUser spotifyControllerUser, ILocalStorageService localStorage)
    {
        _spotifyControllerUser = spotifyControllerUser;
        _localStorage = localStorage;

        var serviceType = ServiceType.Spotify;
        _localStorageKeyInfo = GetLocalStorageKey(serviceType, LocalStorageKey.UserInfo);
        _localStorageKeyCredentials = GetLocalStorageKey(serviceType, LocalStorageKey.UserCredentials);
        _localStorageKeyVerifier = GetLocalStorageKey(serviceType, LocalStorageKey.LoginVerifier);
    }

    public async Task SaveUser()
    {
        var user = _spotifyControllerUser.GetUser();
        if (user?.Info is null || user.Credentials is null)
        {
            return;
        }

        await _localStorage.SetItemAsync(_localStorageKeyInfo, user.Info);
        await _localStorage.SetItemAsync(_localStorageKeyCredentials, user.Credentials);
    }

    public async Task<SpotifyUser?> GetSavedUser()
    {
        var info = await _localStorage.GetItemAsync<SpotifyUserInfo>(_localStorageKeyInfo);
        var credentials = await _localStorage.GetItemAsync<SpotifyUserCredentials>(_localStorageKeyCredentials);

        if (info is null || credentials is null)
        {
            return null;
        }
        var user = new SpotifyUser(info, credentials);
        return user;
    }

    public async Task DeleteSavedUser()
    {
        await _localStorage.RemoveItemAsync(_localStorageKeyInfo);
        await _localStorage.RemoveItemAsync(_localStorageKeyCredentials);
        await DeleteLoginVerifier();
    }

    public async Task SaveLoginVerifier(string loginVerifier)
    {
        await _localStorage.SetItemAsync(_localStorageKeyVerifier, loginVerifier);
    }

    public async Task<string> GetLoginVerifier()
    {
        var codeVerifier = await _localStorage.GetItemAsync<string>(_localStorageKeyVerifier);
        return codeVerifier;
    }

    public async Task DeleteLoginVerifier()
    {
        await _localStorage.RemoveItemAsync(_localStorageKeyVerifier);
    }
}