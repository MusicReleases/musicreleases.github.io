using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars;

public partial class SidebarSection : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;

	[Inject]
	private ILoaderService LoaderService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required SidebarComponent SidebarType { get; set; }

	[Parameter, EditorRequired]
	public required RenderFragment Header { get; set; }

	[Parameter, EditorRequired]
	public required RenderFragment Filter { get; set; }

	[Parameter, EditorRequired]
	public required RenderFragment ChildContent { get; set; }

	[Parameter]
	public string? ContentAriaLabel { get; set; }


	private string SidebarTypeString => SidebarType.ToLowerString();

	private string LoadingText => $"Loading {SidebarType.ToFriendlyString()}...";

	private MobileMenuButtonComponent? MobileMenuType => SidebarType switch
	{
		SidebarComponent.Artists => MobileMenuButtonComponent.Artists,
		SidebarComponent.Date => MobileMenuButtonComponent.Date,
		_ => null
	};

	private bool Loading => SidebarType switch
	{
		SidebarComponent.Artists => LoaderService.IsLoading(LoadingType.Artists),
		SidebarComponent.Date => LoaderService.IsLoading(LoadingType.Artists) || LoaderService.IsLoading(LoadingType.Releases),
		SidebarComponent.Playlists => LoaderService.IsLoading(LoadingType.Playlists) || LoaderService.IsLoading(LoadingType.PlaylistTracks),
		_ => throw new NotImplementedException(),
	};

	private string ClassShow => MobileService.MobileMenu == MobileMenuType ? "show" : string.Empty;


	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
		LoaderService.LoadingStateChanged += StateChanged;
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		LoaderService.LoadingStateChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
