using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Spotify.Settings;
using JakubKastner.SpotifyApi.User;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Buttons;

public partial class UserButton : IDisposable
{
	[Inject]
	private ISpotifyUserClient SpotifyUserClient { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Inject]
	private ISpotifySettingsService SettingsService { get; set; } = default!;


	[Parameter]
	public string? Class { get; set; }


	private string ButtonClass => $"user {Class}";


	private SpotifyUserInfo? _spotifyUser;

	protected override void OnInitialized()
	{
		SettingsService.OnChange += StateChanged;

		SetUser();
	}

	public void Dispose()
	{
		SettingsService.OnChange -= StateChanged;
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
			_spotifyUser = SpotifyUserClient.GetUserRequired().Info;
		}
	}
}
