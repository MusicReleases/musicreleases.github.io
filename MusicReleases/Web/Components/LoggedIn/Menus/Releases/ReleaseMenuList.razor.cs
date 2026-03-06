using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ReleaseMenuList : IDisposable
{
	[Inject]
	private ISpotifyFilterServiceOld SpotifyFilterService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ReleaseMenuComponent MenuType { get; set; }

	[Parameter]
	public bool Hidden { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string ListClass => $"menu-list {MenuType.ToLowerString()}{IsHidden.ToCssClass("hidden")} {Class}";

	private string ReleaseButtonClass => $"{_buttonClass}{(MenuType == ReleaseMenuComponent.Primary ? string.Empty : "-overflow")}";

	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string OverflowListItemClass => $"more {SpotifyFilterService.Filter?.ReleaseType.ToLowerString()}-more";

	private string OverflowButtonTitle => $"{(IsOverflowMenuDisplayed ? "Hide" : "Show")} more";

	private LucideIcon OverflowIcon => IsOverflowMenuDisplayed ? LucideIcon.X : LucideIcon.Ellipsis;

	private bool IsHidden => Hidden || PopupService.IsAnyPopupDisplayed;


	private const string _buttonClass = "menu-releases";

	private const OverflowMenuType _overflowMenu = OverflowMenuType.Releases;


	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += StateChanged;
		OverflowMenuService.OnDisplayChanged += StateChanged;
		PopupService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= StateChanged;
		OverflowMenuService.OnDisplayChanged -= StateChanged;
		PopupService.OnChange -= StateChanged;
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
