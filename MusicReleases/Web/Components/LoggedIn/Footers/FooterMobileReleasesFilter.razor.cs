namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Footers;

public partial class FooterMobileReleasesFilter
{
	private bool _displayMore = false;

	private void DisplayMore()
	{
		_displayMore = !_displayMore;
	}
}
