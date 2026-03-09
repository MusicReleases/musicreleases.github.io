using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Date;

public partial class DateSidebarYearButton : IDisposable
{

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required SortedSet<int> Months { get; set; }


	private bool IsFilterActive => SpotifyReleaseFilterService.Filter.Year == Year;

	private LucideIcon Icon => _showMonths ? LucideIcon.ChevronUp : LucideIcon.ChevronDown;

	private string MonthListClass => _showMonths.ToCssClass(string.Empty, "hidden");

	private string ActiveClass => _showMonths.ToCssClass("active");


	private const string _buttonClass = "sidebar-content date-year";

	private bool _renderMonths = false;

	private bool _showMonths = false;


	protected override void OnParametersSet()
	{
		// display active months and hide others
		_renderMonths = IsFilterActive;
		_showMonths = IsFilterActive;
	}

	protected override void OnInitialized()
	{
		SpotifyReleaseFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ToggleMonths()
	{
		if (!_renderMonths)
		{
			_renderMonths = true;
		}
		_showMonths = !_showMonths;
	}

	private async Task FilterYear()
	{
		SpotifyReleaseFilterService.FilterYear(Year);

		MobileService.HideMenu();
	}
}