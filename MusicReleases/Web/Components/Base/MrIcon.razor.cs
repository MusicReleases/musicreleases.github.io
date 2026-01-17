using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrIcon<TIcon> where TIcon : Enum
{
	[Parameter, EditorRequired]
	public required TIcon Icon { get; set; }
	[Parameter]
	public string? Class { get; set; }
	[Parameter]
	public string Size { get; set; } = "1.25rem";

	private string _svgContent = default!;

	protected override void OnParametersSet()
	{
		SetSvg();
	}

	private void SetSvg()
	{
		_svgContent = IconService.GetSvg(Icon);
	}
}
