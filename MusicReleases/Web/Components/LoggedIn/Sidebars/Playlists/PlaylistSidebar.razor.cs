using JakubKastner.MusicReleases.Spotify.Playlists;
using JakubKastner.SpotifyApi.Playlists;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebar : IDisposable
{
	[Inject]
	private ISpotifyPlaylistState State { get; set; } = default!;

	[Inject]
	private ISpotifyPlaylistFilterService FilterService { get; set; } = default!;


	[Parameter]
	public SpotifyPlaylistType PlaylistTypeFilter { get; set; } = SpotifyPlaylistType.Editable;


	private IReadOnlySet<SpotifyPlaylist>? _playlists;

	private string _searchText = string.Empty;


	protected override void OnInitialized()
	{
		State.OnChange += StateChanged;
	}

	public void Dispose()
	{
		State.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	protected override void OnParametersSet()
	{
		Recalculate();
	}

	private void SearchTextChanged(string newSearchText)
	{
		_searchText = newSearchText;
		Recalculate();
	}

	private void StateChanged()
	{
		Recalculate();
		InvokeAsync(StateHasChanged);
	}

	private void Recalculate()
	{
		_playlists = FilterService.GetFilteredPlaylists(PlaylistTypeFilter, _searchText);
	}
}
