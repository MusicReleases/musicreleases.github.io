using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarButton : IDisposable
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlServiceOld SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required string ArtistId { get; set; }

	[Parameter, EditorRequired]
	public required string ArtistName { get; set; }


	private bool FilterActive => SpotifyReleaseFilterService.Filter?.Artist == ArtistId;


	private const string _buttonClass = "sidebar-content";


	protected override void OnInitialized()
	{
		SpotifyReleaseFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task FilterArtist()
	{
		SpotifyReleaseFilterService.FilterArtist(ArtistId);

		/*var artistIdFilter = FilterActive ? null : ArtistId;

		var url = await SpotifyFilterUrlService.GetFilterUrl(artistIdFilter);
		NavManager.NavigateTo(url);

		MobileService.HideMenu();*/
	}
}