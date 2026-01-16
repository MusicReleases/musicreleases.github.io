using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonClearFilter
{
	[Parameter, EditorRequired]
	public required MenuType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private string ButtonTitle => Type == MenuType.Releases ? "Default filter" : "Clear filter";
	public LucideIcon Icon => LucideIcon.Funnel; // LucideIcon.FunnelX

	private async Task ClearFilter()
	{
		var url = await SpotifyFilterUrlService.ClearFilter(Type);
		NavManager.NavigateTo(url);
	}
}