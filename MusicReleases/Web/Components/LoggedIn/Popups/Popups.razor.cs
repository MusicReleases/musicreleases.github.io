using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Popups;

public partial class Popups : IDisposable
{
	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private IMobileService MobileService { get; set; } = default!;

	private string PopupClass => $"popup mobile-{MobileService.MobileMenu.ToLowerString()}";


	protected override void OnInitialized()
	{
		PopupService.OnChange += StateChanged;
		MobileService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		PopupService.OnChange -= StateChanged;
		MobileService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
