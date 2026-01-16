using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonSort
{
	[Parameter, EditorRequired]
	public MenuType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	public LucideIcon Icon => LucideIcon.ArrowDownUp; // LucideIcon.AZ  // LucideIcon.ZA

	private void Sort()
	{
	}
}