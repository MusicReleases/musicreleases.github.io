using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class Logo
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter]
	public bool DisplayLogoMiddle { get; set; } = false;

	[Parameter]
	public bool ClickEnabled { get; set; } = false;

	[Parameter]
	public string? Class { get; set; }


	private string LogoClass => $"{Class} {(ClickEnabled ? "clickable" : string.Empty)}";
	private string Title => ClickEnabled ? "Go to homepage" : string.Empty;


	private void LogoOnClick()
	{
		if (!ClickEnabled)
		{
			return;
		}

		NavManager.NavigateTo("/");
	}
}
