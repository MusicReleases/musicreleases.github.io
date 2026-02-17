using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrIcon<TIcon> where TIcon : Enum
{
	[Inject]
	private IIconService IconService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required TIcon Icon { get; set; }

	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public string? Title { get; set; }

	[Parameter]
	public bool StopPropagation { get; set; } = false;

	[Parameter]
	public bool Fill { get; set; } = false;

	[Parameter]
	public bool Spin { get; set; } = false;

	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }

	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }


	private string IconClass => $"icon-wrapper{(Fill ? " icon-fill" : "")}{(Spin ? " spin" : string.Empty)} {Class}";


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
