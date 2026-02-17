using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus;

public partial class MainMenu
{
	[Parameter]
	public string? Class { get; set; }

	private bool _displaySettingsMenu = false;

	private void DisplaySettingsMenu(bool displayMenu)
	{
		_displaySettingsMenu = displayMenu;
	}
}
