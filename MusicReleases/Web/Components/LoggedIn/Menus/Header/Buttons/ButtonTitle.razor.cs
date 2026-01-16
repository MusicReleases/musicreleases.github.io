using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonTitle
{
	[Parameter, EditorRequired]
	public MenuType Type { get; set; }

	[Parameter]
	public bool DisplayTitle { get; set; } = true;

	[Parameter]
	public RenderFragment? ChildContent { get; set; }
	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }

	private string Title => DisplayTitle ? Type.ToString() : string.Empty;
}