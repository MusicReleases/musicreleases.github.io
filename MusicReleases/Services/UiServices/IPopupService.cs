using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IPopupService
{
	bool IsAnyPopupDisplayed { get; }
	PopupType? DisplayedPopup { get; }

	event Action? OnChange;

	void Hide();
	bool IsPopupDisplayed(PopupType popupType);
	Task Show(PopupType popupType);
	void SyncClose();
	void SyncFromUrl(PopupType popupType);
	Task Toggle(PopupType popupType);
	Task<bool> UrlChanged();
}