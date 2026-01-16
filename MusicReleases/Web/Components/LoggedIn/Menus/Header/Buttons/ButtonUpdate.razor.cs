using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonUpdate
{
	[Parameter, EditorRequired]
	public MenuType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private void Update()
	{
		var serviceType = ApiLoginService.GetServiceType();

		if (serviceType == ServiceType.Spotify)
		{
			if (SpotifyFilterService.Filter is null)
			{
				throw new NullReferenceException(nameof(SpotifyFilterService.Filter));
			}

			SpotifyWorkflowService.Update(Type, SpotifyFilterService.Filter.ReleaseType);
		}
	}
}