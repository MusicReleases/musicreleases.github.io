using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class MenuSettings
{
    private SpotifyUserInfo? _spotifyUser;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var serviceType = _apiLoginController.GetServiceType();
        if (serviceType == ServiceType.Spotify)
        {
            _spotifyUser = _spotifyControllerUser.GetUserRequired().Info;
        }
    }

    private void LogoutUser()
    {
        _loginController.LogoutUser();
    }
}
