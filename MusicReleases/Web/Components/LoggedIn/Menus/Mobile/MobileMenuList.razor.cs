using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Mobile;

public partial class MobileMenuList : IDisposable
{
	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string OverflowButtonTitle => $"{(IsOverflowMenuDisplayed ? "Hide" : "Show")} menu";

	private LucideIcon OverflowIcon => IsOverflowMenuDisplayed ? LucideIcon.X : LucideIcon.Menu;

	private bool IsTasksPopupDisplayed => PopupService.IsPopupDisplayed(PopupType.Tasks);


	private const OverflowMenuType _overflowMenu = OverflowMenuType.Settings;

	private const string _buttonClass = "menu-mobile";


	protected override void OnInitialized()
	{
		OverflowMenuService.OnDisplayChanged += StateChanged;
		PopupService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		OverflowMenuService.OnDisplayChanged -= StateChanged;
		PopupService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ToggleOverflowMenu()
	{
		OverflowMenuService.ShowOrHideMenu(_overflowMenu);
	}
}
