using Blazored.LocalStorage;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyLoginStorageService(ILocalStorageService localStorageService) : ISpotifyLoginStorageService
{
	private const ServiceType serviceType = ServiceType.Spotify;

	private readonly ILocalStorageService _localStorageService = localStorageService;

	private readonly string _localStorageKeyLoggedInUser = GetLocalStorageKey(serviceType, LocalStorageKey.LoggedInUser);
	private readonly string _localStorageKeyVerifier = GetLocalStorageKey(serviceType, LocalStorageKey.LoginVerifier);

	public async Task SaveUserId(string userId)
	{
		await _localStorageService.SetItemAsync(_localStorageKeyLoggedInUser, userId);
	}

	public async Task<string?> GetSavedUserId()
	{
		var userId = await _localStorageService.GetItemAsync<string>(_localStorageKeyLoggedInUser);
		return userId;
	}

	public async Task DeleteSavedUser()
	{
		var localStorageKeys = GetAllLocalStorageKeys(serviceType);
		foreach (var localStorageKey in localStorageKeys)
		{
			await _localStorageService.RemoveItemAsync(localStorageKey);
		}
	}

	public async Task SaveLoginVerifier(string loginVerifier)
	{
		await _localStorageService.SetItemAsync(_localStorageKeyVerifier, loginVerifier);
	}

	public async Task<string?> GetLoginVerifier()
	{
		var codeVerifier = await _localStorageService.GetItemAsync<string>(_localStorageKeyVerifier);
		return codeVerifier;
	}

	public async Task DeleteLoginVerifier()
	{
		await _localStorageService.RemoveItemAsync(_localStorageKeyVerifier);
	}
}