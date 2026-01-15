using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class MenuSettings
{
	private SpotifyUserInfo? _spotifyUser;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var serviceType = ApiLoginService.GetServiceType();
		if (serviceType == ServiceType.Spotify)
		{
			_spotifyUser = SpotifyUserService.GetUserRequired().Info;
		}
	}
}
