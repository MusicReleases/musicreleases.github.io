using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class ButtonDateYear
{
	[Parameter, EditorRequired]
	public int? Year { get; set; }

	[Parameter, EditorRequired]
	public ISet<int>? Months { get; set; }

	private bool _renderMonths = false;
	private bool _showMonths = false;
	private bool _yearFilter = false;
	private string IconClass => _showMonths ? "fa-angle-up" : "fa-angle-down";
	private string? ButtonClass => _yearFilter ? "active" : string.Empty;
	private string? MonthsClass => _showMonths ? string.Empty : "hidden";

	private void DisplayMonths()
	{
		if (!_renderMonths && !_showMonths)
		{
			_renderMonths = true;
		}
		_showMonths = !_showMonths;
	}
	private void FilterYear()
	{
		_yearFilter = !_yearFilter;
	}
}