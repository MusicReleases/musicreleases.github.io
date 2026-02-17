using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class MobileService : IMobileService
{
	public DisplayMobile DisplayMobile { get; private set; } = DisplayMobile.Releases;

	public event Action? OnDisplayChanged;

	public void ShowMenu(DisplayMobile menuType)
	{
		if (DisplayMobile == menuType)
		{
			// switch to defalt menu type (hide menu)
			//menuType = DisplayMobile.Releases;
		}
		DisplayMobile = menuType;
		OnDisplayChanged?.Invoke();
	}
}
