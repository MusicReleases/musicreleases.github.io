using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class ReleasesActions
{
	private bool _displayTitle = true;
	private readonly MenuButtonsType _type = MenuButtonsType.Releases;
	private void DisplayTitle(bool displayTitle)
	{
		_displayTitle = displayTitle;
	}
}
