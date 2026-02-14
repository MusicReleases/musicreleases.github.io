using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Footers;

public partial class FooterMobileMenu
{
	private string MenuTitle => _displayMenu ? "Hide menu" : "Show menu";

	private LucideIcon Icon => _displayMenu ? LucideIcon.X : LucideIcon.Menu;


	private bool _displayMenu = false;


	private void ToggleMenu()
	{
		_displayMenu = !_displayMenu;
	}

}
