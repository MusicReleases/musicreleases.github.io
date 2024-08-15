using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class Artists
{
	private bool _displayTitle = true;
	private readonly MenuButtonsType _type = MenuButtonsType.Artists;


	private void DisplayTitle(bool displayTitle)
	{
		_displayTitle = displayTitle;
	}
}
