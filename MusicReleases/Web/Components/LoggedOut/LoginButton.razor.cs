using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedOut;

public partial class LoginButton
{
	/// <summary>
	/// Service type (Spotify, Apple Music, ...)
	/// </summary>
	[Parameter]
	public ServiceType Type { get; set; }

	private string TypeName => Type.ToString();

	private Enum TypeIcon => EnumIconsExtensions.GetIconForServiceType(Type);

	/// <summary>
	/// User clicked to login button.
	/// </summary>
	private void ButtonLoginUser()
	{
		LoginService.LoginUser();
	}
}