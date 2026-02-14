using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class FilterClearButton
{
	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public required MenuType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string Class { get; set; } = "rounded-l transparent";


	private string ButtonTitle => Type == MenuType.Releases ? "Default filter" : "Clear filter";


	private readonly LucideIcon _icon = LucideIcon.FunnelX;


	private async Task ClearFilter()
	{
		var url = await SpotifyFilterUrlService.ClearFilter(Type);
		NavManager.NavigateTo(url);
	}
}