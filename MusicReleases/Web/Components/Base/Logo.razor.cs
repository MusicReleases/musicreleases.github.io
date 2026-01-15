using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class Logo
{
	[Parameter]
	public bool DisplayMusic { get; set; } = false;
	[Parameter]
	public bool ClickEnabled { get; set; } = false;
	private string CssClass => ClickEnabled ? "clickable" : string.Empty;

	private void LogoOnClick()
	{
		if (!ClickEnabled)
		{
			return;
		}

		NavManager.NavigateTo("/");
	}
}
