using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Buttons;

public partial class ShowMenuButton
{
	[Parameter, EditorRequired]
	public required Enums.DisplayMobile MenuType { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private string MenuTypeString => MenuType.ToString().ToLower();
	private string Title => (IsMenuDisplayed ? "Hide " : "Show ") + MenuTypeString + " menu";
	private bool IsMenuDisplayed => MobileService.DisplayMobile == MenuType;

	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += OnDisplayChanged;
		base.OnInitialized();
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= OnDisplayChanged;
	}

	private void OnDisplayChanged()
	{
		InvokeAsync(StateHasChanged);
	}


	private void ToggleMenu()
	{
		MobileService.ShowMenu(MenuType);
	}
}
