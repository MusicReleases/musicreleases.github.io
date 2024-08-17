using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class MenuPlaylists
{
	private SpotifyUserList<SpotifyPlaylist>? _playlists => _stateSpotifyPlaylists.Value.List;
	private bool _error => _stateSpotifyPlaylists.Value.Error;
	private bool _loading => _stateSpotifyPlaylists.Value.LoadingAny();

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var userLoggedIn = _apiLoginController.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}
	}
}