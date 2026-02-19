using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class LogoutButton
{
	[Inject]
	private ILoginService LoginService { get; set; } = default!;


	[Parameter]
	public string? Class { get; set; }


	private async Task LogoutUser()
	{
		await LoginService.LogoutUser();
	}
}
