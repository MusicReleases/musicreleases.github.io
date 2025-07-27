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
	private bool YearFilter => SpotifyFilterService.Filter.Year == Year;

	protected override void OnInitialized()
	{
		// display active months on init
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
		base.OnInitialized();

		_renderMonths = YearFilter;
		_showMonths = YearFilter;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
	}

	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

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
		var url = SpotifyFilterUrlService.GetFilterUrl(yearFilter);
		NavManager.NavigateTo(url);
	}
}