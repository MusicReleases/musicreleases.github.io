using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseFilter : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	protected override void OnInitialized()
	{
		SpotifyReleaseFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.Dispose();
		SpotifyReleaseFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
