using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class LayoutMain : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	private string ClassMain => MobileService.MobileMenu.ToString().ToLower();

	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;

		if (!CheckLoggedInUser())
		{
			return;
		}
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
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