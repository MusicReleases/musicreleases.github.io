using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IMobileService
{
	DisplayMobile DisplayMobile { get; }

	event Action? OnDisplayChanged;

	string GetMenuClass(DisplayMobile menuType);
	void ShowMenu(DisplayMobile menuType);
}