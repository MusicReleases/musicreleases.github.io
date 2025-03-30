using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class ButtonDateMonth
{
	[Parameter, EditorRequired]
	public int? Year { get; set; }

	[Parameter, EditorRequired]
	public int? Month { get; set; }

	private bool _monthFilter = false;
	private string? ButtonClass => _monthFilter ? " active" : string.Empty;

	private void FilterMonth()
	{
		_monthFilter = !_monthFilter;
	}
}