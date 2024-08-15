using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

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