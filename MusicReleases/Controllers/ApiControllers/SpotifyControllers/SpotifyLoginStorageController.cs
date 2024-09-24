using Blazored.LocalStorage;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyLoginStorageController(ILocalStorageService localStorage) : ISpotifyLoginStorageController
{
	private const ServiceType serviceType = ServiceType.Spotify;

	private readonly ILocalStorageService _localStorage = localStorage;


	private readonly string _localStorageKeyLoggedInUser = GetLocalStorageKey(serviceType, LocalStorageKey.LoggedInUser);
	private readonly string _localStorageKeyVerifier = GetLocalStorageKey(serviceType, LocalStorageKey.LoginVerifier);

	public async Task SaveUserId(string userId)
	{
		await _localStorage.SetItemAsync(_localStorageKeyLoggedInUser, userId);
	}

	public async Task<string?> GetSavedUserId()
	{
		var userId = await _localStorage.GetItemAsync<string>(_localStorageKeyLoggedInUser);
		return userId;
	}

	public async Task DeleteSavedUser()
	{
		var localStorageKeys = GetAllLocalStorageKeys(serviceType);
		foreach (var localStorageKey in localStorageKeys)
		{
			await _localStorage.RemoveItemAsync(localStorageKey);
		}
	}

	public async Task SaveLoginVerifier(string loginVerifier)
	{
		await _localStorage.SetItemAsync(_localStorageKeyVerifier, loginVerifier);
	}

	public async Task<string?> GetLoginVerifier()
	{
		var codeVerifier = await _localStorage.GetItemAsync<string>(_localStorageKeyVerifier);
		return codeVerifier;
	}

	public async Task DeleteLoginVerifier()
	{
		await _localStorage.RemoveItemAsync(_localStorageKeyVerifier);
	}
}