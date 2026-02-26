using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IPopupService
{
	bool IsAnyPopupDisplayed { get; }
	PopupType? DisplayedPopup { get; }

	event Action? OnChange;

	void Hide();
	bool IsPopupDisplayed(PopupType popupType);
	void Show(PopupType popupType);
	void Toggle(PopupType popupType);
	bool UrlChanged();
}