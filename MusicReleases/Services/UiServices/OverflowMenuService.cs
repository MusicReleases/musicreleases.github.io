using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class OverflowMenuService : IOverflowMenuService
{
	public OverflowMenu? DisplayedMenu { get; private set; } = null;

	public event Action? OnDisplayChanged;

	public bool IsDisplayed(OverflowMenu menuType)
	{
		return DisplayedMenu == menuType;
	}

	public void HideMenu()
	{
		ShowMenuInternal();
	}

	public void ShowMenu(OverflowMenu menuType)
	{
		ShowMenuInternal(menuType);
	}

	public void ShowOrHideMenu(OverflowMenu menuType)
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

	private void ShowMenuInternal(OverflowMenu? menuType = null)
	{
		if (menuType == DisplayedMenu)
		{
			return;
		}

		DisplayedMenu = menuType;
		OnDisplayChanged?.Invoke();
	}
}
