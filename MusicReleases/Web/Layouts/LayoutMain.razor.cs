namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class LayoutMain
{
	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!_apiLoginController.IsUserLoggedIn())
		{
			_navManager.NavigateTo("");
			return;
		}
	}
}