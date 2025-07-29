using JakubKastner.MusicReleases.Base;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IMobileService
{
	DisplayMobile DisplayMobile { get; }

	event Action? OnDisplayChanged;

	string GetMenuClass(Enums.DisplayMobile menuType);
	void ShowMenu(Enums.DisplayMobile menuType);
}