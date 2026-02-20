using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Date;

public partial class DateSidebarMonthButton : IDisposable
{
	[Inject]
	public NavigationManager NavManager { get; set; } = default!;

	[Inject]
	public ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	public ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required int Month { get; set; }


	private bool IsFilterActive => SpotifyFilterService.Filter is not null && SpotifyFilterService.Filter.Month.HasValue && SpotifyFilterService.Filter.Month.Value.Year == Year && SpotifyFilterService.Filter.Month.Value.Month == Month;


	private const string _buttonClass = "sidebar-content date-month";


	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task FilterMonth()
	{
		int? monthFilter = IsFilterActive ? null : Month;
		int? yearFilter = IsFilterActive ? null : Year;

		var url = await SpotifyFilterUrlService.GetFilterUrl(yearFilter, monthFilter);
		NavManager.NavigateTo(url);

		MobileService.HideMenu();
	}
}