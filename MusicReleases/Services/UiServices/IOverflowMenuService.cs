using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IOverflowMenuService
{
	OverflowMenu? DisplayedMenu { get; }

	event Action? OnDisplayChanged;

	bool IsDisplayed(OverflowMenu menuType);
	void HideMenu();
	void ShowMenu(OverflowMenu menuType);
	void ShowOrHideMenu(OverflowMenu menuType);
}