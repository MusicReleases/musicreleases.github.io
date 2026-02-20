using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class Logo
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public LogoComponent LogoType { get; set; }


	private string Class => LogoType.ToLowerString();

	private bool DisplayLogoMiddle => LogoType == LogoComponent.Public;

	private bool ClickEnabled => LogoType == LogoComponent.Public;

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
