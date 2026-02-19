using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IMobileService
{
	MobileMenuButtonComponent MobileMenu { get; }

	event Action? OnDisplayChanged;

	void HideMenu();
	void ShowMenu(MobileMenuButtonComponent menuType);
}