using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ReleaseMenu : IDisposable
{
	[Inject]
	public IMobileService MobileService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;


	private string ClassShow => (MobileService.MobileMenu == MobileMenuButtonComponent.Releases).ToCssClass("show");

	private bool IsOverflowMenuDisplayed => OverflowMenuService.IsDisplayed(_overflowMenu);


	private const OverflowMenuType _overflowMenu = OverflowMenuType.Releases;

	private bool _renderOverflowMenu = false;


	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
		OverflowMenuService.OnDisplayChanged += OverflowMenuDisplayChanged;
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		OverflowMenuService.OnDisplayChanged -= OverflowMenuDisplayChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void OverflowMenuDisplayChanged()
	{
		if (!_renderOverflowMenu && IsOverflowMenuDisplayed)
		{
			_renderOverflowMenu = true;
		}
		StateChanged();
	}
}