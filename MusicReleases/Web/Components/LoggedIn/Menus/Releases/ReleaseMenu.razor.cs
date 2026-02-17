namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ReleaseMenu
{
	private string ClassMore => _showMore ? string.Empty : "hidden";

	private bool _renderMore = false;
	private bool _showMore = false;


	private void DisplayMore(bool showMore)
	{
		if (!_renderMore)
		{
			_renderMore = showMore;
			_showMore = showMore;
			return;
		}

		_showMore = showMore;
	}
}