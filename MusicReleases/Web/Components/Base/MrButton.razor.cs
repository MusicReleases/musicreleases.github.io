using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrButton
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }

	[Parameter]
	public string Type { get; set; } = "button";

	[Parameter]
	public bool Disabled { get; set; }

	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public string? Title { get; set; }
	[Parameter]
	public string? Text { get; set; }
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }
}
