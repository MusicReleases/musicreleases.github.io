using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IMobileService
{
	MobileMenu MobileMenu { get; }

	event Action? OnDisplayChanged;

	void HideMenu();
	void ShowMenu(MobileMenu menuType);
}