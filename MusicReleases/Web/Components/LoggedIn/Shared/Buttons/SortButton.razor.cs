using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class SortButton
{
	[Parameter, EditorRequired]
	public SortButtonComponent ButtonType { get; set; }

	[Parameter]
	public string? Text { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string ButtonClass => $"sort {ButtonType.ToLowerString()} {Class}";

	private string ButtonTitle => $"Sort {ButtonType.ToFriendlyString()}";

	private LucideIcon Icon => LucideIcon.ArrowDownUp; // LucideIcon.AZ  // LucideIcon.ZA


	private void Sort()
	{
	}
}