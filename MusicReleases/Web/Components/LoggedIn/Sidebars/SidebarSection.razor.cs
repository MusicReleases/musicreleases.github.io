using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars;

public partial class SidebarSection
{
	[Parameter, EditorRequired]
	public SidebarType Type { get; set; }

	[Parameter, EditorRequired]
	public bool Loading { get; set; }

	[Parameter, EditorRequired]
	public RenderFragment Header { get; set; }

	[Parameter, EditorRequired]
	public RenderFragment Filter { get; set; }

	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; }

	[Parameter]
	public string? ContentAriaLabel { get; set; }


	private string TypeString => Type.ToString().ToLower();
}
