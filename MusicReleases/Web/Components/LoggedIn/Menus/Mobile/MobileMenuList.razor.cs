using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Mobile;

public partial class MobileMenuList : IDisposable
{
	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;


	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string OverflowButtonTitle => $"{(IsOverflowMenuDisplayed ? "Hide" : "Show")} menu";

	private string OverflowButtonClass => $"menu-mobile {(IsOverflowMenuDisplayed ? " active" : string.Empty)}";

	private LucideIcon OverflowIcon => IsOverflowMenuDisplayed ? LucideIcon.X : LucideIcon.Menu;


	private const OverflowMenu _overflowMenu = OverflowMenu.Settings;


	protected override void OnInitialized()
	{
		OverflowMenuService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		OverflowMenuService.OnDisplayChanged -= StateChanged;
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
