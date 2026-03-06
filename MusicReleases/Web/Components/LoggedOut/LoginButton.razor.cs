using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedOut;

public partial class LoginButton
{
	[Inject]
	private ILoginService LoginService { get; set; } = default!;


	/// <summary>
	/// Service type (Spotify, Apple Music, ...)
	/// </summary>
	[Parameter, EditorRequired]
	public required ServiceType ServiceType { get; set; }


	private string ButtonText => $"Login via {ServiceType.ToFriendlyString(true)}";

	private Enum ServiceTypeIcon => EnumIconsExtensions.GetIconForServiceType(ServiceType);


	/// <summary>
	/// User clicked to login button.
	/// </summary>
	private async Task Login()
	{
		await LoginService.LoginUser();
	}
}