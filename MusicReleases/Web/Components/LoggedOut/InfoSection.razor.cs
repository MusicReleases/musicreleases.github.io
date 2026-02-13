using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedOut;

public partial class InfoSection
{
	[Parameter]
	public string? Title { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}
