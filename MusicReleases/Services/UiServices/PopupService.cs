using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Spotify.Settings;
using JakubKastner.MusicReleases.Spotify.Tasks;

namespace JakubKastner.MusicReleases.Services.UiServices;

internal class PopupService(IBackgroundTaskFilterUrlSynchronizer spotifyTaskFilterUrlSynchronizer, ISpotifySettingsService settingsService, INavigationService navService) : IPopupService
{
	private readonly IBackgroundTaskFilterUrlSynchronizer _spotifyTaskFilterUrlSynchronizer = spotifyTaskFilterUrlSynchronizer;

	private readonly ISpotifySettingsService _settingsService = settingsService;

	private readonly INavigationService _navService = navService;


	public event Action? OnChange;

	public bool IsAnyPopupDisplayed => _popupType is not null;

	public PopupType? DisplayedPopup => _popupType;

	private PopupType? _popupType = null;


	public void SyncFromUrl(PopupType popupType)
	{
		if (IsPopupDisplayed(popupType))
		{
			return;
		}

		_popupType = popupType;
		OnChange?.Invoke();
	}

	public void SyncClose()
	{
		if (!IsAnyPopupDisplayed)
		{
			return;
		}

		_popupType = null;
		OnChange?.Invoke();
	}

	public async Task Show(PopupType popupType)
	{
		if (IsPopupDisplayed(popupType))
		{
			return;
		}

		_popupType = popupType;
		OnChange?.Invoke();


		var url = popupType switch
		{
			PopupType.BackgroundTasks => await _spotifyTaskFilterUrlSynchronizer.GetInitUrl(),
			PopupType.Settings => _settingsService.GetInitUrl(),
			_ => throw new NotSupportedException()
		};

		_navService.OpenPopup(url, popupType.ToString());
	}

	public async Task Toggle(PopupType popupType)
	{
		if (IsPopupDisplayed(popupType))
		{
			// close popup
			Hide();
			return;
		}
		await Show(popupType);
	}

	public void Hide()
	{
		if (!IsAnyPopupDisplayed)
		{
			return;
		}

		_popupType = null;
		OnChange?.Invoke();

		_navService.ClosePopup();
	}

	public bool IsPopupDisplayed(PopupType popupType)
	{
		return _popupType == popupType;
	}
}
