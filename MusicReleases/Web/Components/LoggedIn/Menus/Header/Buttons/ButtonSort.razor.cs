using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonSort
{
	[Parameter, EditorRequired]
	public MenuButtonsType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private void Sort()
	{
	}
}