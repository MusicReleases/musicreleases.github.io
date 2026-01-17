using JakubKastner.MusicReleases.Enums;
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
	private LucideIcon Icon => _showMonths ? LucideIcon.ChevronUp : LucideIcon.ChevronDown;
	private string ButtonClass => $"rounded-m transparent{(YearFilter ? " active" : string.Empty)}";
	private string MonthsClass => _showMonths ? string.Empty : "hidden";
	private bool YearFilter => SpotifyFilterService.Filter?.Year == Year;

	protected override void OnInitialized()
	{
		// display active months on init
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;

		_renderMonths = YearFilter;
		_showMonths = YearFilter;
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

	private void DisplayMonths()
	{
		if (!_renderMonths && !_showMonths)
		{
			_renderMonths = true;
		}
		_showMonths = !_showMonths;
	}

	private async Task FilterYear()
	{
		int? yearFilter = YearFilter ? null : Year;
		var url = await SpotifyFilterUrlService.GetFilterUrl(yearFilter);
		NavManager.NavigateTo(url);
	}
}