using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonTitle
{
	[Parameter, EditorRequired]
	public MenuButtonsType Type { get; set; }

	[Parameter, EditorRequired]
	public bool DisplayTitle { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }
	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }


	private string? _title;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		_title = Type.ToString();
	}

	private async Task HandleClick(MouseEventArgs e)
	{
		if (!OnClick.HasDelegate)
		{
			return;
		}
		await OnClick.InvokeAsync(e);
	}
}