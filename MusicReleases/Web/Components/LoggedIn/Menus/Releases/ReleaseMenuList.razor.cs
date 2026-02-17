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
	public MainMenuType MenuType { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string ClassList => $"{MenuType.ToString().ToLower()} {Class}";

	private string ClassButtonRelease => $"main-menu{(MenuType == MainMenuType.Primary ? string.Empty : "-more")}";

	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string TitleButtonOverflow => $"{(IsOverflowMenuDisplayed ? "Hide" : "Show")} more";

	private string ClassButtonOverflow => $"{_buttonClass}{ClassChipOverflow}";

	private string ClassChipOverflow => IsOverflowMenuDisplayed ? " active" : string.Empty;

	private string ClassLiOverflow => $"more {SpotifyFilterService.Filter?.ReleaseType.ToString().ToLower()}-more";

	private LucideIcon IconOverflow => IsOverflowMenuDisplayed ? LucideIcon.X : LucideIcon.Ellipsis;


	private const string _buttonClass = "main-menu rounded-xl fill-width trasparent";

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
