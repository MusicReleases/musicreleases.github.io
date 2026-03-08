using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Date;

public partial class DateSidebarMonthButton : IDisposable
{
	[Inject]
	public ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required int Month { get; set; }


	private bool IsFilterActive => SpotifyReleaseFilterService.Filter.Month.HasValue && SpotifyReleaseFilterService.Filter.Month.Value.Year == Year && SpotifyReleaseFilterService.Filter.Month.Value.Month == Month;


	private const string _buttonClass = "sidebar-content date-month";


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

	private async Task FilterMonth()
	{
		SpotifyReleaseFilterService.FilterMonth(Year, Month);

		MobileService.HideMenu();
	}
}