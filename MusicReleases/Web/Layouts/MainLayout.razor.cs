using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.Spotify.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class MainLayout : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Inject]
	private ISpotifySettingsService SettingsService { get; set; } = default!;

	private string BodyClass => $"mobile-{MobileService.MobileMenu.ToLowerString()} {SettingsService.UserSettings.Theme.ToLowerString()}";

	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
		SettingsService.OnChange += StateChanged;
		NavManager.LocationChanged += OnLocationChanged;

		if (!CheckLoggedInUser())
		{
			return;
		}

		SyncPopupWithUrl();
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		SettingsService.OnChange -= StateChanged;
		NavManager.LocationChanged -= OnLocationChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
	{
		SyncPopupWithUrl();
	}


	private void SyncPopupWithUrl()
	{
		var url = NavManager.ToBaseRelativePath(NavManager.Uri);

		if (url.StartsWith("tasks", StringComparison.OrdinalIgnoreCase))
		{
			PopupService.SyncFromUrl(PopupType.BackgroundTasks);
		}
		else if (url.StartsWith("settings", StringComparison.OrdinalIgnoreCase))
		{
			PopupService.SyncFromUrl(PopupType.Settings);
		}
		else
		{
			PopupService.SyncClose();
		}
	}


	private bool CheckLoggedInUser()
	{
		if (!ApiLoginService.IsUserLoggedIn())
		{
			NavManager.NavigateTo("");
			return false;
		}
		return true;
	}
}