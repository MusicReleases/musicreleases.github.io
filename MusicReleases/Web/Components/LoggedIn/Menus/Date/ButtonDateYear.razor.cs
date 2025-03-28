using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class ButtonDateYear
{
	[Parameter, EditorRequired]
	public int? Year { get; set; }

	[Parameter, EditorRequired]
	public ISet<int>? Months { get; set; }
}