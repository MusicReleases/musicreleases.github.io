using JakubKastner.SpotifyApi.Base.Objects;

namespace JakubKastner.MusicReleases.Spotify.Settings;

internal interface ISpotifySettingsService
{
	SpotifySettings UserSettings { get; }

	event Action? OnChange;

	string GetInitUrl();
	string GetUrl(string appUrl, string webUrl);
	string GetUrl(SpotifyIdNameUrlObject spotifyUrlObject);
	string GetUrlTitle(string name);
	Task Initialize();
	Task NotifyStateChanged();
	void Search(string searchText);
}