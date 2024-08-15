using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class ButtonTitle
{
	[Parameter, EditorRequired]
	public MenuButtonsType Type { get; set; }

	[Parameter, EditorRequired]
	public bool DisplayTitle { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private string? _title;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		_title = Type.ToString();
	}

	private void Hide()
	{
	}
}