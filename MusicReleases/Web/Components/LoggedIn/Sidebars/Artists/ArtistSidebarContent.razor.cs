using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarContent : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	private ISet<SpotifyArtist>? Artists => SpotifyReleaseFilterService.FilteredArtists;


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