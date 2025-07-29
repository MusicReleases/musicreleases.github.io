using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class MobileService : IMobileService
{
	private readonly string _mobileHideClass = "hide-mobile";
	public DisplayMobile DisplayMobile { get; private set; } = DisplayMobile.Releases;

	public event Action? OnDisplayChanged;

	public string GetMenuClass(DisplayMobile menuType)
	{
		if (DisplayMobile == menuType)
		{
			return string.Empty;
		}
		return _mobileHideClass;
	}

	public void ShowMenu(DisplayMobile menuType)
	{
		if (DisplayMobile == menuType)
		{
			// switch to defalt menu type (hide menu)
			menuType = DisplayMobile.Releases;
		}
		DisplayMobile = menuType;
		OnDisplayChanged?.Invoke();
	}
}
