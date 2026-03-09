using JakubKastner.MusicReleases.Services.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Date;

public partial class DateSidebarContent : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	private Dictionary<int, SortedSet<int>>? FilteredYearMonth => SpotifyReleaseFilterService.FilteredDate;


	protected override void OnInitialized()
	{
		SpotifyReleaseFilterService.OnDataFiltered += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.OnDataFiltered -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
