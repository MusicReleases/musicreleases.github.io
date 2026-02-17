using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class MobileService : IMobileService
{
	public MobileMenu MobileMenu { get; private set; } = _defaultMenu;

	public event Action? OnDisplayChanged;


	private const MobileMenu _defaultMenu = MobileMenu.Releases;

	public void HideMenu()
	{
		ShowMenuInternal();
	}

	public void ShowMenu(MobileMenu menuType)
	{
		ShowMenuInternal(menuType);
	}

	private void ShowMenuInternal(MobileMenu menuType = _defaultMenu)
	{
		if (MobileMenu == menuType)
		{
			return;
		}

		MobileMenu = menuType;
		OnDisplayChanged?.Invoke();
	}
}
