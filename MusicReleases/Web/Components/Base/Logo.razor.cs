using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class Logo
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public LogoComponent Type { get; set; }


	private string Class => Type.ToLowerString();

	private bool DisplayLogoMiddle => Type == LogoComponent.Public;

	private bool ClickEnabled => Type == LogoComponent.Public;

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
