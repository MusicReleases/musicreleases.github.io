using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
	private SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? _artists => _stateSpotifyArtists.Value.List;
	private bool _error => _stateSpotifyArtists.Value.Error;
	private bool _loading => _stateSpotifyArtists.Value.LoadingAny();

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