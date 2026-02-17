using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Mobile;

public partial class MobileMenuList : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;


	[Parameter]
	public EventCallback<bool> OnMoreClick { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string TitleButtonOverflow => $"{(IsOverflowMenuDisplayed ? "Hide" : "Show")} menu";

	private string ClassButtonOverflow => $"{_buttonClass}{(IsOverflowMenuDisplayed ? " active" : string.Empty)}";

	private LucideIcon IconOverflow => IsOverflowMenuDisplayed ? LucideIcon.X : LucideIcon.Menu;


	private const string _buttonClass = "mobile-menu fill-width trasparent";

	private const OverflowMenu _overflowMenu = OverflowMenu.Mobile;


	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
		OverflowMenuService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		OverflowMenuService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private string GetButtonClass(Enums.MobileMenu menuType)
	{
		var activeClass = menuType == MobileService.MobileMenu ? " active" : string.Empty;

		return $"{_buttonClass}{activeClass}";
	}

	public void DisplayMenu(Enums.MobileMenu menuType)
	{
		OverflowMenuService.HideMenu();
		MobileService.ShowMenu(menuType);
	}

	private void ToggleOverflowMenu()
	{
		OverflowMenuService.ShowOrHideMenu(_overflowMenu);
	}
}
