using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class ButtonDateYear
{
	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required SortedSet<int> Months { get; set; }

	private bool _renderMonths = false;
	private bool _showMonths = false;
	private string IconClass => _showMonths ? "fa-angle-up" : "fa-angle-down";
	private string? ButtonClass => YearFilter ? "active" : string.Empty;
	private string? MonthsClass => _showMonths ? string.Empty : "hidden";
	private bool YearFilter => SpotifyFilterState.Value.Filter.Year == Year;

	private void DisplayMonths()
	{
		if (!_renderMonths && !_showMonths)
		{
			_renderMonths = true;
		}
		_showMonths = !_showMonths;
	}
	private void FilterYear()
	{
		int? yearFilter = YearFilter ? null : Year;
		var url = SpotifyFilterService.GetFilterUrl(yearFilter);
		NavManager.NavigateTo(url);
	}
}