using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class ClearFilterButton
{
	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ClearFilterButtonComponent ButtonType { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string ButtonClass => $"filter-clear {ButtonType.ToLowerString()} {Class}";

	private string ButtonText => ButtonType == ClearFilterButtonComponent.All ? "Clear all filters" : string.Empty;

	private string ButtonTitle => ButtonType switch
	{
		ClearFilterButtonComponent.All => "Clear all filters",
		ClearFilterButtonComponent.Artists => "Clear artist filter",
		ClearFilterButtonComponent.Date => "Clear date filter",
		ClearFilterButtonComponent.Releases => "Default releases filter",
		_ => throw new NotImplementedException(),
	};


	private const LucideIcon _icon = LucideIcon.FunnelX;


	private async Task ClearFilter()
	{
		var url = await SpotifyFilterUrlService.ClearFilter(ButtonType);
		NavManager.NavigateTo(url);
	}
}