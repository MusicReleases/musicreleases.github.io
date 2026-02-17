using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Mobile;

public partial class MobileMenuList
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter]
	public EventCallback<bool> OnMoreClick { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string TitleMenu => $"{(_showMenu ? "Hide" : "Show")} menu";
	private string ClassMenu => $"{_buttonClass}{(_showMenu ? " active" : string.Empty)}";


	private LucideIcon IconMenu => _showMenu ? LucideIcon.X : LucideIcon.Menu;

	private DisplayMobile _menuType = DisplayMobile.Releases;

	private const string _buttonClass = "mobile-menu fill-width trasparent";

	private bool _showMenu = false;


	private string GetButtonClass(DisplayMobile menuType)
	{
		var activeClass = menuType == _menuType ? " active" : string.Empty;

		return $"{_buttonClass}{activeClass}";
	}

	private void DisplayReleases()
	{
		_menuType = DisplayMobile.Releases;
		MobileService.ShowMenu(_menuType);
	}

	private void DisplayArtists()
	{
		_menuType = DisplayMobile.Artists;
		MobileService.ShowMenu(_menuType);
	}

	private void DisplayDate()
	{
		_menuType = DisplayMobile.Date;
		MobileService.ShowMenu(_menuType);
	}

	public void DisplayMenu()
	{
		_showMenu = !_showMenu;

		OnMoreClick.InvokeAsync(_showMenu);
	}
}
