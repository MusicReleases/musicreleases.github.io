using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class UpdateButton
{
	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Inject]
	private ISpotifyWorkflowService SpotifyWorkflowService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;


	[Parameter, EditorRequired]
	public MenuType Type { get; set; }

	[Parameter]
	public bool Loading { get; set; } = false;

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private LucideIcon Icon => Loading ? LucideIcon.LoaderCircle : LucideIcon.RefreshCcw;

	private string ButtonClass => $"rounded-l transparent {Class}";


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