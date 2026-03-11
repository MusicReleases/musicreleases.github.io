using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Tasks
{
	[Inject]
	private IBackgroundTaskFilterUrlSynchronizer SpotifyTaskFilterUrlSynchronizer { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


	[Parameter]
	[SupplyParameterFromQuery]
	public string? Filter { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? Search { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		await LoadFilters();
		ShowPopup();
	}

	private async Task LoadFilters()
	{
		await SpotifyTaskFilterUrlSynchronizer.SetFilterFromUrl(Filter, Search);
	}

	private void ShowPopup()
	{
		PopupService.Show(PopupType.BackgroundTasks);
	}
}
