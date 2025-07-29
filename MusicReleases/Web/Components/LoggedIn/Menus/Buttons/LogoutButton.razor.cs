namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Buttons;

public partial class LogoutButton
{

	private void LogoutUser()
	{
		LoginService.LogoutUser();
	}
}
