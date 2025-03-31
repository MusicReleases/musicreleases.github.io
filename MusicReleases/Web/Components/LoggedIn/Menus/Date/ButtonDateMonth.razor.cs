using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class ButtonDateMonth
{
	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required int Month { get; set; }

	private string? ButtonClass => MonthFilter ? " active" : string.Empty;
	private bool MonthFilter => SpotifyFilterState.Value.Filter.Month.HasValue && SpotifyFilterState.Value.Filter.Month.Value.Year == Year && SpotifyFilterState.Value.Filter.Month.Value.Month == Month;

	private void FilterMonth()
	{
		int? monthFilter = MonthFilter ? null : Month;
		int? yearFilter = MonthFilter ? null : Year;

		var url = SpotifyFilterService.GetFilterUrl(yearFilter, monthFilter);
		NavManager.NavigateTo(url);
	}
}