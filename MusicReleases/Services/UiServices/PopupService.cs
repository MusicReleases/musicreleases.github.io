using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Spotify.Settings;
using JakubKastner.MusicReleases.Spotify.Tasks;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.UiServices;

internal class PopupService(IBackgroundTaskFilterUrlSynchronizer spotifyTaskFilterUrlSynchronizer, ISpotifySettingsService settingsService, INavigationService navService, NavigationManager nav) : IPopupService
{
	private readonly IBackgroundTaskFilterUrlSynchronizer _spotifyTaskFilterUrlSynchronizer = spotifyTaskFilterUrlSynchronizer;

	private readonly ISpotifySettingsService _settingsService = settingsService;

	private readonly INavigationService _navService = navService;
	private readonly NavigationManager _nav = nav;


	public event Action? OnChange;

	public bool IsAnyPopupDisplayed => _popupType is not null;

	public PopupType? DisplayedPopup => _popupType;


	public string? _lastUrl;

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
		//await ChangePopup(popupType);
	}

	public void Hide()
	{
		if (!IsAnyPopupDisplayed)
		{
			return;
		}

		_popupType = null;
		OnChange?.Invoke();
		//await ChangePopup();

		_navService.ClosePopup();
	}

	public bool IsPopupDisplayed(PopupType popupType)
	{
		return _popupType == popupType;
	}

	private async Task ChangePopup(PopupType? popupType = null)
	{
		if (popupType is null)
		{
			// close popup

			var lastUrl = _lastUrl;

			if (lastUrl.IsNullOrEmpty())
			{
				_navService.Navigate("/", false, true, "ClosePopup");
			}
			else
			{
				_navService.Navigate(lastUrl, false, true, "ClosePopup");
			}
			return;
		}

		// show popup
		var currentUrl = _navService.Current;
		if (currentUrl.StartsWith("releases"))
		{
			_lastUrl = "/" + currentUrl;
		}

		var url = popupType switch
		{
			PopupType.BackgroundTasks => await _spotifyTaskFilterUrlSynchronizer.GetInitUrl(),
			PopupType.Settings => _settingsService.GetInitUrl(),
			_ => throw new NotSupportedException(nameof(ChangePopup)),
		};

		_navService.Navigate(url, false, true, $"ShowPopup:{popupType}");
	}

	public async Task<bool> UrlChanged()
	{
		var currentUrl = new Uri(_nav.Uri).PathAndQuery;

		var changed = _lastUrl != currentUrl;
		if (!changed)
		{
			// when current url is the same when popup was displayed, then hide popup
			Hide();
		}
		_lastUrl = null;
		return changed;
	}
}
