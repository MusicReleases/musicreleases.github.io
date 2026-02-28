using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Tasks : IDisposable
{
	[Inject]
	private ISpotifyTaskFilterUrlSynchronizer SpotifyTaskFilterUrlSynchronizer { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


	[Parameter]
	[SupplyParameterFromQuery]
	public string? Filter { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? Search { get; set; }


	public void Dispose()
	{
		SpotifyTaskFilterUrlSynchronizer.Dispose();
		GC.SuppressFinalize(this);
	}

	protected override void OnParametersSet()
	{
		LoadFilters();
		ShowPopup();
	}

	private void LoadFilters()
	{
		SpotifyTaskFilterUrlSynchronizer.SetFilterFromUrl(Filter, Search);
	}

	private void ShowPopup()
	{
		PopupService.Show(PopupType.Tasks);
	}
}
