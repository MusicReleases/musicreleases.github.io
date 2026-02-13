using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Main.Settings;

public partial class SettingsMenu
{
	[Inject]
	private ISpotifyApiUserService SpotifyUserService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;


	private SpotifyUserInfo? _spotifyUser;


	protected override void OnInitialized()
	{
		SetUser();
	}

	private void SetUser()
	{
		var serviceType = ApiLoginService.GetServiceType();
		if (serviceType == ServiceType.Spotify)
		{
			_spotifyUser = SpotifyUserService.GetUserRequired().Info;
		}
	}
}
