using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrLoading
{
	[Parameter, EditorRequired]
	public required bool Loading { get; set; }

	[Parameter]
	public string Text { get; set; } = "Loading...";
}
