using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class ButtonDateMonth
{
	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required int Month { get; set; }

	private string ButtonClass => $"rounded-m transparent{(MonthFilter ? " active" : string.Empty)}";
	private bool MonthFilter => SpotifyFilterService.Filter is not null && SpotifyFilterService.Filter.Month.HasValue && SpotifyFilterService.Filter.Month.Value.Year == Year && SpotifyFilterService.Filter.Month.Value.Month == Month;

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
		GC.SuppressFinalize(this);
	}

	private void OnFilterOrDataChanged()
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