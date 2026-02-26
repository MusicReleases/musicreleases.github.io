using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Popups;

public partial class TaskPopup : IDisposable
{
	[Inject]
	private ISpotifyTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	private ICollection<SpotifyBackgroundTask> DisplayedTasks => SpotifyTaskManagerService.FilteredTasks;

	protected override void OnInitialized()
	{
		SpotifyTaskManagerService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		SpotifyTaskManagerService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

}
