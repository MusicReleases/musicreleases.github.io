namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class MenuSettings
{
    protected override void OnInitialized()
    {

    }

    private void LogoutUser()
    {
        _loginController.LogoutUser();
    }
}
