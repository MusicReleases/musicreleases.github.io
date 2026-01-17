using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonSort
{
	[Parameter, EditorRequired]
	public MenuType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private LucideIcon Icon => LucideIcon.ArrowDownUp; // LucideIcon.AZ  // LucideIcon.ZA
	private string Title => $"Sort {Type.ToString().ToLower()}";

	private void Sort()
	{
	}
}