using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class NewButton
{
	[Parameter, EditorRequired]
	public MenuType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private void New()
	{
	}
}