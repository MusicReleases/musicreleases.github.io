using JakubKastner.MusicReleases.Spotify.Settings.User;
using JakubKastner.SpotifyApi.Base.Objects;

namespace JakubKastner.MusicReleases.Spotify.Settings;

internal class SpotifySettingsService(ISpotifyUserSettingsDbService dbService) : ISpotifySettingsService
{
	private readonly ISpotifyUserSettingsDbService _dbService = dbService;

	public event Action? OnChange;


	public SpotifySettings UserSettings { get; private set; } = new();


	private const string _baseUrl = "/settings";


	public string GetInitUrl()
	{
		return _baseUrl;
	}

	public async Task Initialize()
	{
		// TODO cancel token
		UserSettings = await _dbService.Get(default) ?? new();
		OnChange?.Invoke();
	}

	public async Task NotifyStateChanged()
	{
		OnChange?.Invoke();
		await SaveToDb();
	}

	private async Task SaveToDb()
	{
		// TODO cancel token
		await _dbService.Save(UserSettings, true, default);
	}

	public void Search(string searchText)
	{
		// TODO settings - search
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
