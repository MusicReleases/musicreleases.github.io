using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ReleaseMenuButton : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Parameter, EditorRequired]
	public required MainReleasesType ReleaseType { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private bool IsActive => SpotifyReleaseFilterService.Filter.ReleaseType == ReleaseType;

	private string ButtonText => ReleaseType.ToFriendlyString(true);

	private string ButtonTitle => $"View {ReleaseType.ToFriendlyString()}";

	private LucideIcon Icon => EnumIconsExtensions.GetIconForRelease(ReleaseType);


	protected override void OnInitialized()
	{
		SpotifyReleaseFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task DisplayReleases()
	{
		SpotifyReleaseFilterService.FilterReleaseType(ReleaseType);
	}
}
