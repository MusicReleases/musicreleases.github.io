using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ReleaseMenuButton : IDisposable
{
	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ReleaseType ReleaseType { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private bool IsActive => SpotifyFilterService.Filter?.ReleaseType == ReleaseType;

	private string ButtonText => ReleaseType.ToFriendlyString(true);

	private string ButtonTitle => $"View {ReleaseType.ToFriendlyString()}";

	private LucideIcon Icon => EnumIconsExtensions.GetIconForRelease(ReleaseType);


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
