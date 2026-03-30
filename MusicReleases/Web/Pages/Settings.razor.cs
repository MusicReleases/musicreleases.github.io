using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Settings
{
	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	// TODO section url param
	[Parameter]
	public string? Section { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		await ShowPopup();
	}

	private async Task ShowPopup()
	{
		await PopupService.Show(PopupType.Settings);
	}
}
