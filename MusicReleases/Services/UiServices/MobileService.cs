using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class MobileService : IMobileService
{
	public MobileMenuButtonComponent MobileMenu { get; private set; } = _defaultMenu;

	public event Action? OnDisplayChanged;


	private const MobileMenuButtonComponent _defaultMenu = MobileMenuButtonComponent.Releases;

	public void HideMenu()
	{
		ShowMenuInternal();
	}

	public void ShowMenu(MobileMenuButtonComponent menuType)
	{
		ShowMenuInternal(menuType);
	}

	private void ShowMenuInternal(MobileMenuButtonComponent menuType = _defaultMenu)
	{
		if (MobileMenu == menuType)
		{
			return;
		}

		MobileMenu = menuType;
		OnDisplayChanged?.Invoke();
	}
}
