using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebar : IDisposable
{
	[Inject]
	public ILoaderService LoaderService { get; set; } = default!;

	[Inject]
	public ISpotifyFilterPlaylistService FilterService { get; set; } = default!;


	[Parameter]
	public PlaylistType TypeFilter { get; set; } = PlaylistType.Editable;


	private List<SpotifyPlaylist>? FilteredPlaylists => FilterService.GetFilteredPlaylists(_searchText, TypeFilter)?.ToList();

	private bool Loading => LoaderService.IsLoading(LoadingType.Playlists) || LoaderService.IsLoading(LoadingType.PlaylistTracks);


	private string _searchText = string.Empty;


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateChanged;
		FilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateChanged;
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
