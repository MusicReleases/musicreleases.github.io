using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IOverflowMenuService
{
	OverflowMenuType? DisplayedMenu { get; }

	event Action? OnDisplayChanged;

	bool IsDisplayed(OverflowMenuType menuType);
	void HideMenu();
	void ShowMenu(OverflowMenuType menuType);
	void ShowOrHideMenu(OverflowMenuType menuType);
}