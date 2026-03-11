using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarContent : IDisposable
{
	[Inject]
	private ISpotifyArtistFilterService SpotifyArtistFilterService { get; set; } = default!;


	private ISet<SpotifyArtist>? Artists => SpotifyArtistFilterService.FilteredArtists;


	protected override void OnInitialized()
	{
		SpotifyArtistFilterService.OnSearchOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyArtistFilterService.OnSearchOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}