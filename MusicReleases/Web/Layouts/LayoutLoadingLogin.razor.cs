namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class LayoutLoadingLogin
{
    // TODO loading page
    protected override async Task OnInitializedAsync()
    {
        if (_spotifyControllerUser.IsLoggedIn())
        {
            return;
        }

        // get token from url
        var currentUrl = _navManager.Uri;
        var user = await _spotifyControllerUser.LoginUser(currentUrl);
        if (user)
        {
            // navigate to releases page
            // TODO change release type
            _navManager.NavigateTo("releases/albums");
        }
        else
        {
            // user is not logged in (error)
            _navManager.NavigateTo("");
        }
    }
}