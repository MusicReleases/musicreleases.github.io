using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class PopupService(NavigationManager navigationManager) : IPopupService
{
	private readonly NavigationManager _navigationManager = navigationManager;


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

	public void Toggle(PopupType popupType)
	{
		if (IsPopupDisplayed(popupType))
		{
			// close popup
			Hide();
			return;
		}

		ChangePopup(popupType);
	}

	public void Hide()
	{
		if (!IsAnyPopupDisplayed)
		{
			return;
		}

		_popupType = null;
		ChangePopup();
		OnChange?.Invoke();
	}

	public bool IsPopupDisplayed(PopupType popupType)
	{
		return _popupType == popupType;
	}

	private void ChangePopup(PopupType? popupType = null)
	{
		if (popupType is null)
		{
			// close popup

			if (_lastUrl.IsNullOrEmpty())
			{
				_navigationManager.NavigateTo("/");
			}
			else
			{
				_navigationManager.NavigateTo(_lastUrl, false);
			}
			return;
		}

		// show popup

		_lastUrl = new Uri(_navigationManager.Uri).PathAndQuery;

		switch (popupType)
		{
			case PopupType.Tasks:
				_navigationManager.NavigateTo("/tasks", false);
				break;
			default:
				throw new NotSupportedException(nameof(ChangePopup));
		}
	}

	public bool UrlChanged()
	{
		var currentUrl = new Uri(_navigationManager.Uri).PathAndQuery;

		var changed = _lastUrl != currentUrl;
		if (!changed)
		{
			// when current url is the same when popup was displayed, then hide popup
			Hide();
		}

		return changed;
	}
}
