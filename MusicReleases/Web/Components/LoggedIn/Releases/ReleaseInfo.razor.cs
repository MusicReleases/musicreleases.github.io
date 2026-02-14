using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseInfo
{
	[Inject]
	private IDragDropService DragDropService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Parameter]
	public required SpotifyRelease SpotifyRelease { get; set; }


	private string ReleaseTypeTitle => $"{SpotifyRelease.ReleaseType} release";


	private void OnDragStart()
	{
		DragDropService.StartDrag(SpotifyRelease, DragDropType.Release);
	}

	private async Task FilterMonth()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(SpotifyRelease.ReleaseDate.Year, SpotifyRelease.ReleaseDate.Month);
		NavManager.NavigateTo(url);
	}

	private async Task FilterYear()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(SpotifyRelease.ReleaseDate.Year);
		NavManager.NavigateTo(url);
	}
}
