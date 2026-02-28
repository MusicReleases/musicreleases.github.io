using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IPopupService
{
	bool IsAnyPopupDisplayed { get; }
	PopupType? DisplayedPopup { get; }

	event Action? OnChange;

	Task Hide();
	bool IsPopupDisplayed(PopupType popupType);
	void Show(PopupType popupType);
	Task Toggle(PopupType popupType);
	Task<bool> UrlChanged();
}