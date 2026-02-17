using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Settings;

public partial class SettingsMenu : IDisposable
{
	[Inject]
	private ISpotifyApiUserService SpotifyUserService { get; set; } = default!;


	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;


	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);

	private string ClassOverflowMenu => IsOverflowMenuDisplayed ? "show" : string.Empty;


	private const OverflowMenu _overflowMenu = OverflowMenu.Mobile;

	private SpotifyUserInfo? _spotifyUser;


	protected override void OnInitialized()
	{
		SetUser();
		OverflowMenuService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		OverflowMenuService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
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
