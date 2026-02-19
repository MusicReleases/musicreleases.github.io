using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class UserButton
{
	[Inject]
	private ISpotifyApiUserService SpotifyUserService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;


	[Parameter]
	public string? Class { get; set; }


	private string ButtonClass => $"user {Class}";


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
