using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.SpotifyApi.Artists;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarButton : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required SpotifyArtist Artist { get; set; }


	private bool FilterActive => SpotifyReleaseFilterService.Filter?.Artist == Artist.Id;


	private const string _buttonClass = "sidebar-content";


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

	private async Task FilterArtist()
	{
		SpotifyReleaseFilterService.FilterArtist(Artist.Id);
	}
}