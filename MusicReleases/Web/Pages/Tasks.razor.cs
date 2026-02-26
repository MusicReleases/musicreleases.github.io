using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Tasks
{
	[Inject]
	private ISpotifyTaskFilterUrlService SpotifyTaskFilterUrlService { get; set; } = default!;

	[Inject]
	private ISpotifyTaskFilterService SpotifyTaskFilterService { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Parameter]
	[SupplyParameterFromQuery]
	public string? Filter { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? Search { get; set; }


	protected override void OnParametersSet()
	{
		LoadFilters();
		ShowPopup();
	}

	private void LoadFilters()
	{
		var filter = SpotifyTaskFilterUrlService.ParseFilterFromUrlParams(Filter);
		SpotifyTaskFilterService.SetFilterAndSearch(filter, Search);
	}

	private void ShowPopup()
	{
		PopupService.Show(PopupType.Tasks);
	}
}
