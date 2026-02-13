using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class SortButton
{
	[Parameter, EditorRequired]
	public MenuType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private string Title => $"Sort {Type.ToString().ToLower()}";
	private LucideIcon Icon => LucideIcon.ArrowDownUp; // LucideIcon.AZ  // LucideIcon.ZA


	private void Sort()
	{
	}
}