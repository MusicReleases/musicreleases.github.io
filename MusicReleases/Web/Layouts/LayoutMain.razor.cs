namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class LayoutMain
{
	protected override void OnInitialized()
	{
		if (!CheckLoggedInUser())
		{
			return;
		}
		base.OnInitialized();
	}

	private bool CheckLoggedInUser()
	{
		if (!_apiLoginController.IsUserLoggedIn())
		{
			_navManager.NavigateTo("");
			return false;
		}
		return true;
	}
}