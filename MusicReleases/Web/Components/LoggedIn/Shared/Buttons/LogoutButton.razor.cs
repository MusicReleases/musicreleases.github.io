using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class LogoutButton
{
	[Inject]
	private ILoginService LoginService { get; set; } = default!;

	private void LogoutUser()
	{
		LoginService.LogoutUser();
	}
}
