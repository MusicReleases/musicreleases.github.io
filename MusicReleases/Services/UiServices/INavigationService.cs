namespace JakubKastner.MusicReleases.Services.UiServices;

public interface INavigationService : IDisposable
{
	string CurrentUrl { get; }
	string BaseUrl { get; }
	Uri CurrentUri { get; }

	event Action<string>? Navigated;

	void ClosePopup(string fallback = "/releases");
	void Navigate(string url, bool forceLoad = false, bool replace = false, string? reason = null);
	void OpenPopup(string popupUrl, string reason);
}