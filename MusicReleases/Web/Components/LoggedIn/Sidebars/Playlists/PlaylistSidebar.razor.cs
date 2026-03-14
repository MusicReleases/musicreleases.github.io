using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebar : IDisposable
{

	[Inject]
	public ISpotifyPlaylistFilterService FilterService { get; set; } = default!;


	[Parameter]
	public PlaylistEnums PlaylistTypeFilter { get; set; } = PlaylistEnums.Editable;


	private List<SpotifyPlaylist>? FilteredPlaylists => FilterService.GetFilteredPlaylists(_searchText, PlaylistTypeFilter)?.ToList();


	private string _searchText = string.Empty;


	protected override void OnInitialized()
	{
		FilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		FilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void SearchTextChanged(string newSearchText)
	{
		_searchText = newSearchText;
	}
}
