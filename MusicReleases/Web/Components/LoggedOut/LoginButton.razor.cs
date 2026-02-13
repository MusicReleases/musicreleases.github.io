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
	[Parameter]
	public ServiceType Type { get; set; }


	private string ButtonText => $"Login via {Type}";

	private Enum TypeIcon => EnumIconsExtensions.GetIconForServiceType(Type);


	/// <summary>
	/// User clicked to login button.
	/// </summary>
	private void ButtonOnClick()
	{
		LoginService.LoginUser();
	}
}