using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.SpotifyApi.Artists;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarContent : IDisposable
{
	[Inject]
	private ISpotifyArtistFilterService FilterService { get; init; } = default!;

	private List<SpotifyArtist>? _artists;

	protected override void OnInitialized()
	{
		FilterService.OnChanged += SearchChanged;
		SearchChanged();
	}

	public void Dispose()
	{
		FilterService.OnChanged -= SearchChanged;
		GC.SuppressFinalize(this);
	}

	private Task StateChanged() => InvokeAsync(StateHasChanged);

	private void SearchChanged()
	{
		_artists = FilterService.FilteredArtists is null ? null : [.. FilterService.FilteredArtists];
		_ = StateChanged();
	}
}