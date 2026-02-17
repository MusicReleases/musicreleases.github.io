using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Date;

public partial class DateSidebarYearButton : IDisposable
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required SortedSet<int> Months { get; set; }


	private LucideIcon Icon => _showMonths ? LucideIcon.ChevronUp : LucideIcon.ChevronDown;

	private string ButtonClass => $"date-year sidebar-button rounded-m fill-width transparent{(YearFilter ? " active" : string.Empty)}";

	private string MonthsClass => _showMonths ? string.Empty : "hidden";

	private string ActiveClass => _showMonths ? "active" : string.Empty;

	private bool YearFilter => SpotifyFilterService.Filter?.Year == Year;


	private bool _renderMonths = false;

	private bool _showMonths = false;


	protected override void OnParametersSet()
	{
		// display active months and hide others
		_renderMonths = YearFilter;
		_showMonths = YearFilter;
	}

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

		MobileService.HideMenu();
	}
}