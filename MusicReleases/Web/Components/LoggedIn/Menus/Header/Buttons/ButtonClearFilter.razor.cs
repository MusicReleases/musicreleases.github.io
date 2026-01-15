using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonClearFilter
{
	[Parameter, EditorRequired]
	public required MenuButtonsType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private string ButtonTitle => Type == MenuButtonsType.Releases ? "Default filter" : "Clear filter";

	private async Task ClearFilter()
	{
		var url = await SpotifyFilterUrlService.ClearFilter(Type);
		NavManager.NavigateTo(url);
	}
}