using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarContent : IDisposable
{
	[Inject]
	private ISpotifyArtistFilterService SpotifyArtistFilterService { get; init; } = default!;

	private List<SpotifyArtist>? _artists;

	protected override void OnInitialized()
	{
		SpotifyArtistFilterService.OnDataChanged += SearchChanged;
	}

	public void Dispose()
	{
		SpotifyArtistFilterService.OnDataChanged -= SearchChanged;
		GC.SuppressFinalize(this);
	}

	private Task StateChanged() => InvokeAsync(StateHasChanged);

	private void SearchChanged()
	{
		_artists = SpotifyArtistFilterService.FilteredArtists is null ? null : [.. SpotifyArtistFilterService.FilteredArtists];
		_ = StateChanged();
	}
}