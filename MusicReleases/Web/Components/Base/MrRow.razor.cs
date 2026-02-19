using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrRow
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string? Class { get; set; }
}