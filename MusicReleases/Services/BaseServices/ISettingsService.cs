using JakubKastner.MusicReleases.Objects.User;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISettingsService
{
	UserSettings UserSettings { get; }

	event Action? OnChange;

	string GetInitUrl();
	string GetUrl(string appUrl, string webUrl);
	string GetUrl(SpotifyIdNameUrlObject spotifyUrlObject);
	string GetUrlTitle(string name);
	Task Initialize();
	Task NotifyStateChanged();
	void Search(string searchText);
}