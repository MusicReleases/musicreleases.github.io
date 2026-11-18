using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class Logo
{
	[Inject]
	private INavigationService NavService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required LogoComponent LogoType { get; set; }


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

		NavService.Navigate("/", true, false, "logo clicked");
	}
}
