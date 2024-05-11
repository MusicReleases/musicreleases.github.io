using JakubKastner.MusicReleases.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;
public partial class MenuPlaylists
{
	private SpotifyUserList<SpotifyPlaylist>? _playlists => _stateSpotifyPlaylists.Value.Playlists;
	private bool _loading => _stateSpotifyPlaylists.Value.Loading;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var userLoggedIn = _apiLoginController.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}

		var serviceType = _apiLoginController.GetServiceType();

		if (serviceType == Enums.ServiceType.Spotify)
		{
			_spotifyPlaylistsController.GetPlaylists();

			/*if (_stateSpotifyPlaylists.Value.Initialized == false)
			{
				LoadPlaylists();
				_dispatcher.Dispatch(new SpotifyPlaylistsActionInitialized());
			}*/
		}
	}

	/*private void LoadPlaylists()
	{
		_dispatcher.Dispatch(new SpotifyPlaylistsActionLoad());
	}*/
}