using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Main.Releases;

public partial class ReleaseMenuButton : IDisposable
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;


	[Parameter, EditorRequired]
	public ReleaseType ReleaseType { get; set; }


	private string ReleaseTypeText => ReleaseType.ToString();

	private string ButtonTitle => $"View released {ReleaseTypeText}";

	private string ButtonClass => $"main-menu rounded-xl trasparent{(ReleaseFilter ? " active" : string.Empty)}";

	private LucideIcon Icon => EnumIconsExtensions.GetIconForRelease(ReleaseType);

	private bool ReleaseFilter => SpotifyFilterService.Filter?.ReleaseType == ReleaseType;


	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task DisplayReleases()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(ReleaseType);
		NavManager.NavigateTo(url);
	}
}
