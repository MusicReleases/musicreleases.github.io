namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class MenuPlaylists
{
	/*private SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? _playlists => StateSpotifyPlaylist.Value.List;
	private bool Error => StateSpotifyPlaylist.Value.Error;
	private bool Loading => StateSpotifyPlaylist.Value.LoadingAny();*/

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