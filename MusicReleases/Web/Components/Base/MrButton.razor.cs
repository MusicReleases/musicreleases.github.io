using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrButton
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public RenderFragment? ChildContentAfterText { get; set; }

	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }

	[Parameter]
	public string Type { get; set; } = "button";

	[Parameter]
	public bool Active { get; set; }

	[Parameter]
	public bool Hidden { get; set; }

	[Parameter]
	public bool Loading { get; set; }

	[Parameter]
	public bool Disabled { get; set; }

	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public string? Title { get; set; }

	[Parameter]
	public string? Text { get; set; }

	[Parameter]
	public string? Href { get; set; }

	[Parameter]
	public bool NewTab { get; set; } = true;

	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }


	private string? ButtonTitle => Title.IsNullOrEmpty() ? Text : Title;

	private string ButtonClass => $"button {Class}{Active.ToCssClass()}{Hidden.ToCssClass()}{Loading.ToCssClass()}";

	private string HrefTarget => NewTab ? "_blank" : "_self";

	private string HrefRel => NewTab ? "noopener noreferrer" : string.Empty;
}
