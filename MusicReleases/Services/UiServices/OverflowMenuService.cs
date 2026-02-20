using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class OverflowMenuService : IOverflowMenuService
{
	public OverflowMenuType? DisplayedMenu { get; private set; } = null;

	public event Action? OnDisplayChanged;

	public bool IsDisplayed(OverflowMenuType menuType)
	{
		return DisplayedMenu == menuType;
	}

	public void HideMenu()
	{
		ShowMenuInternal();
	}

	public void ShowMenu(OverflowMenuType menuType)
	{
		ShowMenuInternal(menuType);
	}

	public void ShowOrHideMenu(OverflowMenuType menuType)
	{
		if (IsDisplayed(menuType))
		{
			// hide
			HideMenu();
			return;
		}

		// show
		ShowMenuInternal(menuType);
	}

	private void ShowMenuInternal(OverflowMenuType? menuType = null)
	{
		if (menuType == DisplayedMenu)
		{
			return;
		}

		DisplayedMenu = menuType;
		OnDisplayChanged?.Invoke();
	}
}
