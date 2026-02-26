using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Settings;

public partial class SettingsMenu : IDisposable
{
	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;


	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string ClassShow => IsOverflowMenuDisplayed.ToCssClass("show");


	private const string _buttonClass = "menu-settings";

	private const OverflowMenuType _overflowMenu = OverflowMenuType.Settings;


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
}
