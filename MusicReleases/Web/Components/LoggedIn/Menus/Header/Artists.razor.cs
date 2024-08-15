using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class Artists
{
	private bool _displayTitle = true;
	private MenuButtonsType _type = MenuButtonsType.Artists;


	private void DisplayTitle(bool displaTitle)
	{
		_displayTitle = displaTitle;
	}
}
