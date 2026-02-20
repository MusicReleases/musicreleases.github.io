using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars;

public partial class SidebarTitleButton
{
	[Parameter, EditorRequired]
	public required SidebarComponent SidebarType { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }


	private string ButtonText => SidebarType.ToString();
}