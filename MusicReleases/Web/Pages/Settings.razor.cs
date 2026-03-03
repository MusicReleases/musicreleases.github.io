using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Settings //: IDisposable
{
	/*[Inject]
	private ISpotifyTaskFilterUrlSynchronizer SpotifyTaskFilterUrlSynchronizer { get; set; } = default!;*/

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


	[Parameter]
	public string? Section { get; set; }



	protected override async Task OnParametersSetAsync()
	{
		//await LoadFilters();
		ShowPopup();
	}
	/*public void Dispose()
	{
		//SpotifyTaskFilterUrlSynchronizer.Dispose();
		GC.SuppressFinalize(this);
	}

	private async Task LoadFilters()
	{
		await SpotifyTaskFilterUrlSynchronizer.SetFilterFromUrl(Filter, Search);
	}*/

	private void ShowPopup()
	{
		PopupService.Show(PopupType.Settings);
	}
}
