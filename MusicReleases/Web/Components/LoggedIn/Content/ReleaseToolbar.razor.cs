using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseToolbar
{
	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }


	[Parameter, EditorRequired]
	public required bool Loading { get; set; }


	private readonly MenuType _type = MenuType.Releases;


	private async Task ClearFilter()
	{
		var url = await SpotifyFilterUrlService.ClearFilter();
		NavManager.NavigateTo(url);
	}

	private void AddToPlaylist()
	{

	}

	private void Play()
	{

	}
}
