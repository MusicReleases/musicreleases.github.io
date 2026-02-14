using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarButton : IDisposable
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required string ArtistId { get; set; }

	[Parameter, EditorRequired]
	public required string ArtistName { get; set; }


	private string Class => $"rounded-m fill-width transparent{(FilterActive ? " active" : string.Empty)}";
	private bool FilterActive => SpotifyFilterService.Filter?.Artist == ArtistId;


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

	private async Task FilterArtist()
	{
		var artistIdFilter = FilterActive ? null : ArtistId;

		var url = await SpotifyFilterUrlService.GetFilterUrl(artistIdFilter);
		NavManager.NavigateTo(url);
	}
}