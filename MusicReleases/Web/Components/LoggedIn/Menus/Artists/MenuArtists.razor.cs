using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
	private ISet<SpotifyArtist>? Artists => SpotifyFilterState.Value.FilteredArtists;
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