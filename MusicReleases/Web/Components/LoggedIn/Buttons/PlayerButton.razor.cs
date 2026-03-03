using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Buttons;

public partial class PlayerButton
{
	[Parameter]
	public string? Class { get; set; }
}
