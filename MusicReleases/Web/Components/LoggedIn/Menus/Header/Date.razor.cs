using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class Date
{
	private bool _displayTitle = true;
	private readonly MenuButtonsType _type = MenuButtonsType.Date;

	private void DisplayTitle(bool displayTitle)
	{
		_displayTitle = displayTitle;
	}
}
