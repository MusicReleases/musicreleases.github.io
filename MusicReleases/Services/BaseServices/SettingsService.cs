using JakubKastner.MusicReleases.Objects.User;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SettingsService(IDbSpotifyUserSettingsService dbService, ISpotifyApiUserService spotifyUserService) : ISettingsService
{
	private readonly IDbSpotifyUserSettingsService _dbService = dbService;

	private readonly ISpotifyApiUserService _spotifyUserService = spotifyUserService;


	public event Action? OnChange;


	public UserSettings UserSettings { get; private set; } = new();


	private const string _baseUrl = "/settings";


	public string GetInitUrl()
	{
		return _baseUrl;
	}

	public async Task Initialize()
	{
		var userId = _spotifyUserService.GetUserIdRequired();

		UserSettings = await _dbService.Get(userId);
		OnChange?.Invoke();
	}

	public async Task NotifyStateChanged()
	{
		OnChange?.Invoke();
		await SaveToDb();
	}

	private async Task SaveToDb()
	{
		var userId = _spotifyUserService.GetUserIdRequired();
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
