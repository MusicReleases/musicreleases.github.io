using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Mobile;

public partial class MobileMenuButton : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required MobileMenuButtonComponent ButtonType { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private bool IsActive => ButtonType == MobileService.MobileMenu && !PopupService.IsAnyPopupDisplayed;

	private string ButtonClass => $"menu-mobile {ButtonType.ToLowerString()}";

	private string ButtonTitle => $"View {ButtonType.ToFriendlyString()}";

	private LucideIcon Icon => ButtonType switch
	{
		MobileMenuButtonComponent.Releases => LucideIcon.Music2,
		MobileMenuButtonComponent.Artists => LucideIcon.MicVocal,
		MobileMenuButtonComponent.Date => LucideIcon.Calendar,
		_ => throw new NotImplementedException(),
	};

	private FilterType FilterType => ButtonType switch
	{
		MobileMenuButtonComponent.Releases => FilterType.Advanced,
		MobileMenuButtonComponent.Artists => FilterType.Artist,
		MobileMenuButtonComponent.Date => FilterType.Date,
		_ => throw new NotImplementedException(),
	};

	private bool IsFilterActive => SpotifyReleaseFilterService.IsFilterActive(FilterType);


	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
		OverflowMenuService.OnDisplayChanged += StateChanged;
		SpotifyReleaseFilterService.OnFilterOrDataChanged += StateChanged;
		PopupService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		OverflowMenuService.OnDisplayChanged -= StateChanged;
		SpotifyReleaseFilterService.Dispose();
		SpotifyReleaseFilterService.OnFilterOrDataChanged -= StateChanged;
		PopupService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	public async Task DisplayMenu()
	{
		await PopupService.Hide();
		OverflowMenuService.HideMenu();
		MobileService.ShowMenu(ButtonType);
	}
}
