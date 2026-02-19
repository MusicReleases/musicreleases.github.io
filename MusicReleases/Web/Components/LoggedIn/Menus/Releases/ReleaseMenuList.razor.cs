using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ReleaseMenuList : IDisposable
{
	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;


	[Parameter, EditorRequired]
	public ReleaseMenuComponent MenuType { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string ListClass => $"menu-list {MenuType.ToLowerString()} {Class}";

	private string ReleaseButtonClass => $"{_buttonClass}{(MenuType == ReleaseMenuComponent.Primary ? string.Empty : "-overflow")}";

	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string ClassActiveOverflow => IsOverflowMenuDisplayed ? "active" : string.Empty;

	private string OverflowListItemClass => $"more {SpotifyFilterService.Filter?.ReleaseType.ToLowerString()}-more";

	private string OverflowButtonTitle => $"{(IsOverflowMenuDisplayed ? "Hide" : "Show")} more";

	private string OverflowButtonClass => $"{_buttonClass} {ClassActiveOverflow}";

	private LucideIcon OverflowIcon => IsOverflowMenuDisplayed ? LucideIcon.X : LucideIcon.Ellipsis;


	private const string _buttonClass = "menu-releases";

	private const OverflowMenu _overflowMenu = OverflowMenu.Releases;


	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += StateChanged;
		OverflowMenuService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= StateChanged;
		OverflowMenuService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	public void ToggleOverflowMenu()
	{
		OverflowMenuService.ShowOrHideMenu(_overflowMenu);
	}
}
