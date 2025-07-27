using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class MenuPlaylists
{
	//private SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? _playlists => StateSpotifyPlaylist.Value.List;
	private SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? _playlists => SpotifyPlaylistsService.Playlists;
	//private bool Error => StateSpotifyPlaylist.Value.Error;
	private bool Loading => LoaderService.IsLoading(LoadingType.Playlists);

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
		SpotifyPlaylistsService.OnPlaylistsDataChanged += OnPlaylistsDataChanged;
		base.OnInitialized();

		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		SpotifyPlaylistsService.OnPlaylistsDataChanged -= OnPlaylistsDataChanged;
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
	private void OnPlaylistsDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}