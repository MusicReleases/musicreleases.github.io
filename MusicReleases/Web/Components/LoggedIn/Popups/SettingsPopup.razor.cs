using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.User;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Popups;

public partial class SettingsPopup : IDisposable
{
	[Inject]
	private ISettingsService SettingsService { get; set; } = default!;


	private UserSettings UserSettings => SettingsService.UserSettings;


	private const string _buttonClass = "popup-settings";


	protected override void OnInitialized()
	{
		SettingsService.OnChange += StateChanged;
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

	private void Search(string searchText)
	{
		SettingsService.Search(searchText);
	}

	private async Task ChangeOpenLinksInApp(bool openLinksInApp)
	{
		if (UserSettings.OpenLinksInApp == openLinksInApp)
		{
			return;
		}

		UserSettings.OpenLinksInApp = openLinksInApp;
		await SettingsService.NotifyStateChanged();
	}

	private async Task ChangePlaylistAddToProfile(bool playlistAddToProfile)
	{
		if (UserSettings.PlaylistAddToProfile == playlistAddToProfile)
		{
			return;
		}

		UserSettings.PlaylistAddToProfile = playlistAddToProfile;
		await SettingsService.NotifyStateChanged();
	}

	private async Task ChangePlaylistNewTrackPositionLast(bool playlistNewTrackPositionLast)
	{
		if (UserSettings.PlaylistNewTrackPositionLast == playlistNewTrackPositionLast)
		{
			return;
		}

		UserSettings.PlaylistNewTrackPositionLast = playlistNewTrackPositionLast;
		await SettingsService.NotifyStateChanged();
	}

	private async Task ChangeTheme(Theme theme)
	{
		if (UserSettings.Theme == theme)
		{
			return;
		}

		UserSettings.Theme = theme;
		await SettingsService.NotifyStateChanged();
	}

	private static bool IsActive(bool property, bool value)
	{
		return property == value;
	}

	private static bool IsActive(Theme property, Theme value)
	{
		return property == value;
	}
}
