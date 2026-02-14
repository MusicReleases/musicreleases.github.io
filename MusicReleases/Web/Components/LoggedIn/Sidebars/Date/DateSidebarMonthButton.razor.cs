using JakubKastner.MusicReleases.Services.BaseServices;
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


	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required int Month { get; set; }


	private string ButtonClass => $"rounded-m fill-width transparent{(MonthFilter ? " active" : string.Empty)}";
	private bool MonthFilter => SpotifyFilterService.Filter is not null && SpotifyFilterService.Filter.Month.HasValue && SpotifyFilterService.Filter.Month.Value.Year == Year && SpotifyFilterService.Filter.Month.Value.Month == Month;


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
		int? monthFilter = MonthFilter ? null : Month;
		int? yearFilter = MonthFilter ? null : Year;

		var url = await SpotifyFilterUrlService.GetFilterUrl(yearFilter, monthFilter);
		NavManager.NavigateTo(url);
	}
}