using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;
namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Settings;

public partial class MenuSettingsDisplayButton : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required MobileMenu MenuType { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private string MenuTypeString => MenuType.ToString().ToLower();
	private string Text => (IsMenuDisplayed ? "Hide " : "Show ") + MenuTypeString + " menu";
	private bool IsMenuDisplayed => MobileService.MobileMenu == MenuType;


	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ToggleMenu()
	{
		MobileService.ShowMenu(MenuType);
	}
}
