using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrIcon<TIcon> where TIcon : Enum
{
	[Parameter, EditorRequired]
	public required TIcon Type { get; set; }
	[Parameter]
	public string CssClass { get; set; } = "";
	[Parameter]
	public string Size { get; set; } = "1.25rem";

	private string svgContent = "";

	protected override void OnParametersSet()
	{
		SetSvg();
	}

	private void SetSvg()
	{
		svgContent = Type switch
		{
			LucideIcon lucide => IconService.GetSvg(lucide),
			SpotifyIcon spotify => IconService.GetSvg(spotify),
			CustomIcon custom => IconService.GetSvg(custom),
			_ => throw new ArgumentException($"Icon type '{typeof(TIcon).Name}' is not supported."),
		};
	}
}
