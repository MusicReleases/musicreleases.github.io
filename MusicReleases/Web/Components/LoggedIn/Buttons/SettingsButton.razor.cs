using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Buttons;

public partial class SettingsButton : IDisposable
{
	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;


	[Parameter]
	public string? Class { get; set; }


	private bool IsPopupDisplayed => PopupService.IsPopupDisplayed(_popupType);

	private string ButtonTitle => $"{(IsPopupDisplayed ? "Hide" : "Show")} settings";

	private string ButtonClass => $"settings {Class}";


	private const PopupType _popupType = PopupType.Settings;


	protected override void OnInitialized()
	{
		PopupService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		PopupService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task ViewSettings()
	{
		OverflowMenuService.HideMenu();
		await PopupService.Toggle(_popupType);
	}
}
