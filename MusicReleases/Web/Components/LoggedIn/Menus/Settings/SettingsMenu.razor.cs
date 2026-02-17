using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Settings;

public partial class SettingsMenu
{
	[Inject]
	private ISpotifyApiUserService SpotifyUserService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Parameter]
	public bool DisplayOnMobile { get; set; } = false;

	private string ClassMenu => DisplayOnMobile ? "show" : string.Empty;
	private string TitleButton => _displayMenu ? "Hide menu" : "Show menu";


	private bool _displayMenu = false;
	//private bool _displayMenu = true;

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


	private void ShowMenu()
	{
		_displayMenu = !_displayMenu;
	}
}
