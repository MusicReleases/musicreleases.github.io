using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebar : IDisposable
{

	[Inject]
	public ISpotifyFilterPlaylistService FilterService { get; set; } = default!;


	[Parameter]
	public PlaylistType PlaylistTypeFilter { get; set; } = PlaylistType.Editable;


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
