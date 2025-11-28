using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class LogoShort
{
	[Parameter]
	public bool DisplayMusic { get; set; } = false;

}
