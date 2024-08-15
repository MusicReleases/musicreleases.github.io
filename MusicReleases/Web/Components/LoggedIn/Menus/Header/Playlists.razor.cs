using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class Playlists
{
	private bool _displayTitle = true;
	private MenuButtonsType _type = MenuButtonsType.Playlists;


	private void DisplayTitle(bool displayTitle)
	{
		_displayTitle = displayTitle;
	}
}
