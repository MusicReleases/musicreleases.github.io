using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseSection : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	private string ClassShow => (MobileService.MobileMenu == MobileMenuButtonComponent.Releases).ToCssClass("show");


	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
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
}
