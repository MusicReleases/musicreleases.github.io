using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
	private SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? Artists => SpotifyArtistState.Value.List;
	private bool Error => SpotifyArtistState.Value.Error;
	private bool Loading => SpotifyArtistState.Value.LoadingAny();

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}
	}
}