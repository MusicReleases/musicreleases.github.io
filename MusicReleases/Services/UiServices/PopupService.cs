using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class PopupService(IBackgroundTaskFilterUrlSynchronizer spotifyTaskFilterUrlSynchronizer, ISettingsService settingsService, NavigationManager navManager) : IPopupService
{
	private readonly IBackgroundTaskFilterUrlSynchronizer _spotifyTaskFilterUrlSynchronizer = spotifyTaskFilterUrlSynchronizer;

	private readonly ISettingsService _settingsService = settingsService;

	private readonly NavigationManager _navManager = navManager;


	public event Action? OnChange;

	public bool IsAnyPopupDisplayed => _popupType is not null;

	public PopupType? DisplayedPopup => _popupType;


	public string? _lastUrl;

	private PopupType? _popupType = null;


	public void Show(PopupType popupType)
	{
		_popupType = popupType;
		OnChange?.Invoke();
	}

	public async Task Toggle(PopupType popupType)
	{
		if (IsPopupDisplayed(popupType))
		{
			// close popup
			await Hide();
			return;
		}

		await ChangePopup(popupType);
	}

	public async Task Hide()
	{
		if (!IsAnyPopupDisplayed)
		{
			return;
		}

		_popupType = null;
		await ChangePopup();
		OnChange?.Invoke();
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
				_navManager.NavigateTo("/");
			}
			else
			{
				_navManager.NavigateTo(lastUrl, false);
			}
			return;
		}

		// show popup
		var currentUri = new Uri(_navManager.Uri);
		if (currentUri.AbsolutePath.StartsWith("/releases"))
		{
			_lastUrl = currentUri.PathAndQuery;
		}

		var url = popupType switch
		{
			PopupType.BackgroundTasks => await _spotifyTaskFilterUrlSynchronizer.GetInitUrl(),
			PopupType.Settings => _settingsService.GetInitUrl(),
			_ => throw new NotSupportedException(nameof(ChangePopup)),
		};

		_navManager.NavigateTo(url, false);
	}

	public async Task<bool> UrlChanged()
	{
		var currentUrl = new Uri(_navManager.Uri).PathAndQuery;

		var changed = _lastUrl != currentUrl;
		if (!changed)
		{
			// when current url is the same when popup was displayed, then hide popup
			await Hide();
		}
		_lastUrl = null;
		return changed;
	}
}
