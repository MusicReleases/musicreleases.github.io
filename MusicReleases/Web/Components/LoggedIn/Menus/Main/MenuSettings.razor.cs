using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Main;

public partial class MenuSettings
{
	private SpotifyUserInfo? _spotifyUser;

	protected override void OnInitialized()
	{
		SetUser();
	}

	private void SetUser()
	{
		var serviceType = ApiLoginService.GetServiceType();
		if (serviceType == ServiceType.Spotify)
		{
			_spotifyUser = SpotifyUserService.GetUserRequired().Info;
		}
	}
}
