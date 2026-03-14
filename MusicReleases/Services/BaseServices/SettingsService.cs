using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Objects.User;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services.Api;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SettingsService(IDbSpotifyUserSettingsService dbService, IApiUserClient spotifyUserClient) : ISettingsService
{
	private readonly IDbSpotifyUserSettingsService _dbService = dbService;

	private readonly IApiUserClient _spotifyUserClient = spotifyUserClient;


	public event Action? OnChange;


	public UserSettings UserSettings { get; private set; } = new();


	private const string _baseUrl = "/settings";


	public string GetInitUrl()
	{
		return _baseUrl;
	}

	public async Task Initialize()
	{
		var userId = _spotifyUserClient.GetUserIdRequired();

		UserSettings = await _dbService.Get(userId) ?? new();
		OnChange?.Invoke();
	}

	public async Task NotifyStateChanged()
	{
		OnChange?.Invoke();
		await SaveToDb();
	}

	private async Task SaveToDb()
	{
		var userId = _spotifyUserClient.GetUserIdRequired();
		await _dbService.Save(UserSettings, userId);
	}

	public void Search(string searchText)
	{
		// TODO
	}

	public string GetUrl(string appUrl, string browserUrl)
	{
		return UserSettings.OpenLinksInApp ? appUrl : browserUrl;
	}

	public string GetUrl(SpotifyIdNameUrlObject spotifyUrlObject)
	{
		return GetUrl(spotifyUrlObject.UrlApp, spotifyUrlObject.UrlWeb);
	}

	public string GetUrlTitle(string name)
	{
		return $"Open {name} in {(UserSettings.OpenLinksInApp ? "Spotify application" : "web browser")}";
	}
}
