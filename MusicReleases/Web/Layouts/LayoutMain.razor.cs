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
		if (!ApiLoginService.IsUserLoggedIn())
		{
			NavManager.NavigateTo("");
			return false;
		}
		return true;
	}
}