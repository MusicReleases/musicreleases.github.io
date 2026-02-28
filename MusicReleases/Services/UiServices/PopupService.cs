using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class PopupService(ISpotifyTaskFilterUrlSynchronizer spotifyTaskFilterUrlSynchronizer, NavigationManager navManager) : IPopupService
{
	private readonly ISpotifyTaskFilterUrlSynchronizer _spotifyTaskFilterUrlSynchronizer = spotifyTaskFilterUrlSynchronizer;

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

			if (_lastUrl.IsNullOrEmpty())
			{
				_navManager.NavigateTo("/");
			}
			else
			{
				_navManager.NavigateTo(_lastUrl, false);
			}
			return;
		}

		// show popup

		_lastUrl = new Uri(_navManager.Uri).PathAndQuery;

		switch (popupType)
		{
			case PopupType.BackgroundTasks:
				var url = await _spotifyTaskFilterUrlSynchronizer.GetInitUrl();
				_navManager.NavigateTo(url, false);
				break;
			default:
				throw new NotSupportedException(nameof(ChangePopup));
		}
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

		return changed;
	}
}
