using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrChip
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public bool Active { get; set; }

	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }

	private string ChipClass => $"chip {Class}{Active.ToCssClass()}";
}
